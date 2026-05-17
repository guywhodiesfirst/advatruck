namespace Bot.Services;

using System.Net;
using Bot.Interfaces;
using Bot.Options;
using Core.Exceptions;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

/// <inheritdoc/>
public class TrackingService(
    ITelegramBotClient bot,
    IApiClient api,
    IDriverSessionStore sessions,
    IOptions<TelegramBotOptions> options,
    ILogger<TrackingService> logger)
    : ITrackingService
{
    /// <inheritdoc/>
    public async Task HandleLocationAsync(long chatId, Location location)
    {
        var driverId = await sessions.GetDriverIdByChatIdAsync(chatId);
        if (driverId == null)
        {
            logger.LogWarning("Location update received for unauthenticated chat {ChatId}", chatId);
            return;
        }

        var now = DateTime.UtcNow;
        var lastUpdate = await sessions.GetLastLocationUpdateAsync(driverId.Value);

        if (lastUpdate == null || now - lastUpdate >= TimeSpan.FromMinutes(options.Value.LocationUpdateIntervalMinutes))
        {
            var token = await sessions.GetTokenByDriverIdAsync(driverId.Value);
            if (string.IsNullOrEmpty(token))
            {
                logger.LogError("Token not found for driver {DriverId}", driverId);
                return;
            }

            try
            {
                await api.SendLocationAsync(driverId.Value, location, token);
                await sessions.SaveLastLocationUpdateAsync(driverId.Value, now);

                await UpdateTrackingStatusAsync(chatId, driverId.Value, now);
            }
            catch (TmsException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                logger.LogWarning("Driver {DriverId} unauthorized. Stopping tracking.", driverId);
                await StopTrackingAsync(chatId);
                await bot.SendMessage(chatId, "⚠️ Сесія вичерпана. Будь ласка, увійдіть знову для продовження трекінгу.");
            }
            catch (TmsException ex)
            {
                logger.LogWarning("Business error sending location for {DriverId}: {Message}", driverId, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error sending location for driver {DriverId}", driverId);
            }
        }
    }

    /// <inheritdoc/>
    public async Task StopTrackingAsync(long chatId)
    {
        var driverId = await sessions.GetDriverIdByChatIdAsync(chatId);
        if (driverId == null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        await sessions.SaveTrackingStoppedAtAsync(driverId.Value, now);

        var messageId = await sessions.GetTrackingMessageIdAsync(driverId.Value);
        if (messageId != null)
        {
            try
            {
                await bot.EditMessageText(
                    chatId,
                    messageId.Value,
                    $"⛔ Трекінг зупинено\n🕒 {FormatTimestamp(now)}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to update stop tracking message for chat {ChatId}", chatId);
            }
        }

        await sessions.ClearTrackingAsync(driverId.Value);
    }

    private async Task UpdateTrackingStatusAsync(long chatId, Guid driverId, DateTime time)
    {
        var isTracking = await sessions.IsTrackingActiveAsync(driverId);

        if (!isTracking)
        {
            await StartTrackingAsync(chatId, driverId);
            return;
        }

        var messageId = await sessions.GetTrackingMessageIdAsync(driverId);
        if (messageId == null)
        {
            return;
        }

        try
        {
            await bot.EditMessageText(
                chatId,
                messageId.Value,
                $"📡 Трекінг активний\n⏱ Оновлено: {FormatTimestamp(time)}");
        }
        catch (Exception ex)
        {
            logger.LogDebug("Tracking message update skipped for chat {ChatId}: {Msg}", chatId, ex.Message);
        }
    }

    private async Task StartTrackingAsync(long chatId, Guid driverId)
    {
        await sessions.ClearTrackingStoppedAtAsync(driverId);
        await sessions.SetTrackingActiveAsync(driverId);

        var msg = await bot.SendMessage(
            chatId,
            "📡 Трекінг активний\n🕒 Очікування оновлень...");

        await sessions.SaveTrackingMessageIdAsync(driverId, msg.MessageId);
    }

    private static string FormatTimestamp(DateTime time)
    {
        return time.ToLocalTime().ToString("HH:mm:ss");
    }
}