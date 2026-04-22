namespace Bot.Services;

using Bot.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

// TODO: refactor this class to separate responsibilities

/// <inheritdoc/>
public class TrackingService(
    ITelegramBotClient bot,
    DriverApiClient api,
    IDriverSessionStore sessions,
    ICommandService commands,
    IOptions<TelegramBotOptions> options,
    ILogger<TrackingService> logger)
    : ITrackingService
{
    /// <inheritdoc/>
    public async Task HandleLocationAsync(long chatId, Location location)
    {
        var driverId = await sessions.GetAuthenticatedDriverAsync(chatId);

        if (driverId == null)
        {
            await commands.SendLoginPromptAsync(chatId, "Спочатку увійди");
            return;
        }

        var now = DateTime.UtcNow;

        var lastUpdate = await sessions.GetLastLocationUpdateAsync(driverId.Value);

        var shouldSendToApi =
            lastUpdate == null ||
            now - lastUpdate >= TimeSpan.FromMinutes(options.Value.LocationUpdateIntervalMinutes);

        if (shouldSendToApi)
        {
            await sessions.SaveLastLocationUpdateAsync(driverId.Value, now);
            await api.SendLocationAsync(driverId.Value, location);
            await UpdateTrackingMessageAsync(chatId, now);
        }

        if (location.LivePeriod == null)
        {
            await bot.SendMessage(chatId, "📍 Локацію отримано");
        }
    }

    /// <inheritdoc/>
    public async Task StopTrackingAsync(long chatId)
    {
        var now = DateTime.UtcNow;

        await sessions.SaveTrackingStoppedAtAsync(chatId, now);

        var messageId = await sessions.GetTrackingMessageIdAsync(chatId);

        if (messageId != null)
        {
            try
            {
                await bot.EditMessageText(
                    chatId,
                    messageId.Value,
                    $"⛔ Трекінг зупинено\n🕒 {FormatTimestamp(now)}");
            }
            catch
            {
                logger.LogWarning("Failed to update stop tracking message {ChatId}", chatId);
            }
        }

        await sessions.ClearTrackingAsync(chatId);
    }

    private async Task UpdateTrackingMessageAsync(long chatId, DateTime time)
    {
        var isTracking = await sessions.IsTrackingActiveAsync(chatId);

        if (!isTracking)
        {
            await StartTrackingAsync(chatId);
            return;
        }

        var messageId = await sessions.GetTrackingMessageIdAsync(chatId);

        if (messageId == null)
        {
            return;
        }

        try
        {
            await bot.EditMessageText(
                chatId,
                messageId.Value,
                $"📡 Трекінг активний\n⏱ {FormatTimestamp(time)}");
        }
        catch
        {
            logger.LogWarning("Failed tracking update {ChatId}", chatId);
        }
    }

    private async Task StartTrackingAsync(long chatId)
    {
        await sessions.ClearTrackingStoppedAtAsync(chatId);

        await sessions.SetTrackingActiveAsync(chatId);

        var msg = await bot.SendMessage(
            chatId,
            "📡 Трекінг активний\n🕒 —");

        await sessions.SaveTrackingMessageIdAsync(chatId, msg.MessageId);
    }

    private static string FormatTimestamp(DateTime time)
    {
        return time.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
    }
}