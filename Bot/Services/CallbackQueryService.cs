namespace Bot.Services;

using System.Text;
using Bot.Interfaces;
using Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class CallbackQueryService(
    ITelegramBotClient bot,
    IApiClient apiClient,
    IDriverSessionStore sessions,
    ILogger<CallbackQueryService> logger) : ICallbackQueryService
{
    public async Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var data = callbackQuery.Data ?? string.Empty;

        await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);

        if (data.StartsWith("show_stops:") || data.StartsWith("load_details:"))
        {
            var loadIdStr = data.Split(':')[1];
            if (!Guid.TryParse(loadIdStr, out var loadId))
            {
                return;
            }

            await HandleShowStopsAsync(chatId, loadId, ct);
        }
    }

    private async Task HandleShowStopsAsync(long chatId, Guid loadId, CancellationToken ct)
    {
        var driverId = await sessions.GetDriverIdByChatIdAsync(chatId);
        if (driverId == null)
        {
            logger.LogWarning("DriverId not found for ChatId {ChatId}", chatId);
            return;
        }

        var token = await sessions.GetTokenByDriverIdAsync(driverId.Value);
        if (string.IsNullOrEmpty(token))
        {
            logger.LogWarning("Token not found for DriverId {DriverId}", driverId);
            return;
        }

        try
        {
            var load = await apiClient.GetLoadByIdAsync(loadId, token);
            if (load == null)
            {
                await bot.SendMessage(chatId, "❌ Замовлення не знайдено\\.", parseMode: ParseMode.MarkdownV2, cancellationToken: ct);
                return;
            }

            var sb = new StringBuilder();

            // Екрануємо весь рядок цілком або кожну частину дуже ретельно
            sb.AppendLine($"📦 *Деталі замовлення №{Escape(load.Id.ToString().Split('-')[0])}*");
            sb.AppendLine(Escape("⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯"));
            sb.AppendLine($"⚖️ *Вага:* {Escape(load.CargoWeight.ToString())} {Escape("кг")}");
            sb.AppendLine($"📏 *Габарити:* {Escape($"{load.CargoLength}x{load.CargoWidth}x{load.CargoHeight}")} {Escape("см")}");

            if (!string.IsNullOrEmpty(load.Note))
            {
                sb.AppendLine($"📝 *Нотатка:* {Escape(load.Note)}");
            }

            sb.AppendLine($"👤 *Диспетчер:* {Escape(load.DispatcherName)}");
            sb.AppendLine();

            sb.AppendLine(Escape("📍 Маршрут замовлення:"));
            var sortedStops = load.LoadStops.OrderBy(s => s.Timestamp).ToList();

            foreach (var stop in sortedStops)
            {
                var status = stop.Timestamp <= DateTime.UtcNow ? "✅" : "🕒";

                var timeStr = stop.Timestamp.AddHours(3).ToString("dd.MM HH:mm");
                var type = stop.LoadLocationType == Core.Enums.LoadLocationType.Pickup
                    ? Escape("📦 Завантаження")
                    : Escape("🏁 Розвантаження");

                sb.AppendLine($"{status} *{Escape(timeStr)}* {Escape("|")} {type}");
                sb.AppendLine($"{Escape("└")} {Escape(stop.Address)}");

                if (!string.IsNullOrEmpty(stop.Note))
                {
                    sb.AppendLine($"  _{Escape($"Нотатка: {stop.Note}")}_");
                }

                sb.AppendLine();
            }

            var finalMessage = sb.ToString();

            await bot.SendMessage(
                chatId: chatId,
                text: finalMessage,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching load stops for Load {LoadId}", loadId);
            await bot.SendMessage(chatId, "❌ Помилка завантаження зупинок\\.", parseMode: ParseMode.MarkdownV2, cancellationToken: ct);
        }
    }

    /// <summary>
    /// Escapes characters for Telegram MarkdownV2.
    /// </summary>
    private static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        var reservedCharacters = new[]
        {
            "_", "*", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!",
        };

        foreach (var character in reservedCharacters)
        {
            text = text.Replace(character, "\\" + character);
        }

        return text;
    }
}