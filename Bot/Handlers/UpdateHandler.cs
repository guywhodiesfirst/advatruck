namespace Bot.Handlers;

using Bot.Interfaces;
using Bot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

/// <summary>
/// Handles incoming Telegram updates and routes them to appropriate handlers.
/// </summary>
public class UpdateHandler(
    ITelegramBotClient bot,
    DriverApiClient api,
    IDriverSessionStore sessions)
{
    /// <summary>
    /// Entry point for processing Telegram updates.
    /// </summary>
    /// <param name="update">The incoming Telegram update.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task HandleAsync(Update update)
    {
        if (update.Type != UpdateType.Message && update.Type != UpdateType.EditedMessage)
        {
            return;
        }

        var message = update.Message ?? update.EditedMessage;

        if (message == null)
        {
            return;
        }

        var chatId = message.Chat.Id;

        if (message.Location != null)
        {
            await HandleLocationAsync(chatId, message.Location);
            return;
        }

        if (message.Text is not { } text)
        {
            return;
        }

        if (await sessions.IsAwaitingEmailAsync(chatId))
        {
            await HandleLoginAsync(chatId, text);
            await sessions.ClearAwaitingEmailAsync(chatId);
            return;
        }

        await HandleCommandAsync(chatId, text);
    }

    /// <summary>
    /// Handles text commands from user.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="text">Received text message.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task HandleCommandAsync(long chatId, string text)
    {
        switch (text)
        {
            case "/start":
                await SendStartMessageAsync(chatId);
                return;

            case BotButtons.Login:
                await sessions.MarkAwaitingEmailAsync(chatId);
                await bot.SendMessage(chatId, "Введи email:");
                return;

            case BotButtons.StartTrip:
                await bot.SendMessage(chatId, "Рейс розпочато 🚀", replyMarkup: GetMainKeyboard());
                return;

            case BotButtons.EndTrip:
                await sessions.ClearAuthenticatedDriverAsync(chatId);
                await sessions.ClearTrackingAsync(chatId);

                await bot.SendMessage(
                    chatId,
                    "Рейс завершено ✅",
                    replyMarkup: GetStartKeyboard());
                return;

            case BotButtons.StartTracking:
                await SendTrackingInstructionsAsync(chatId);
                return;

            default:
                await bot.SendMessage(
                    chatId,
                    "Використай кнопки 👇",
                    replyMarkup: GetStartKeyboard());
                return;
        }
    }

    /// <summary>
    /// Handles driver login by email.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="email">Email entered by the user.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task HandleLoginAsync(long chatId, string email)
    {
        var driver = await api.LoginAsync(email);

        if (driver == null)
        {
            await bot.SendMessage(chatId, "❌ Не знайдено користувача");
            return;
        }

        await sessions.SaveAuthenticatedDriverAsync(chatId, driver.Id);

        await bot.SendMessage(
            chatId,
            $"✅ Вітаю, {driver.FirstName}",
            replyMarkup: GetMainKeyboard());
    }

    /// <summary>
    /// Handles incoming location updates.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="location">Location data received from Telegram.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task HandleLocationAsync(long chatId, Location location)
    {
        var driverId = await sessions.GetAuthenticatedDriverAsync(chatId);

        if (driverId == null)
        {
            await SendUnauthorizedAsync(chatId);
            return;
        }

        await api.SendLocationAsync(driverId.Value, location);

        var isLive = location.LivePeriod != null;

        if (!isLive)
        {
            await bot.SendMessage(chatId, "📍 Локацію отримано");
            return;
        }

        var wasTracking = await sessions.IsTrackingActiveAsync(chatId);

        if (!wasTracking)
        {
            await sessions.SetTrackingActiveAsync(chatId);

            var message = await bot.SendMessage(
                chatId,
                "📡 Трекінг активний\n\n⏱ Оновлено: —");

            await sessions.SaveTrackingMessageIdAsync(chatId, message.MessageId);
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
                chatId: chatId,
                messageId: messageId.Value,
                text: $"📡 Трекінг активний\n\n⏱ Оновлено: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        }
        catch
        {
            // Telegram інколи кидає помилки при частих апдейтах
        }
    }

    /// <summary>
    /// Sends welcome message with login button.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SendStartMessageAsync(long chatId)
    {
        await bot.SendMessage(
            chatId,
            "👋 Вітаю!\nНатисни 🔐 Увійти щоб почати",
            replyMarkup: GetStartKeyboard());
    }

    /// <summary>
    /// Sends instructions for starting tracking.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SendTrackingInstructionsAsync(long chatId)
    {
        await bot.SendMessage(
            chatId,
            "📡 Щоб почати трекінг:\n\n" +
            "1️⃣ Натисни '📎'\n" +
            "2️⃣ Обери 'Місце'\n" +
            "3️⃣ Натисни \"Поділитися моїм маячком на мапі\"\n\n" +
            "Бот буде отримувати оновлення автоматично 🚀",
            replyMarkup: GetMainKeyboard());
    }

    /// <summary>
    /// Sends unauthorized response and shows login keyboard.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SendUnauthorizedAsync(long chatId)
    {
        await bot.SendMessage(
            chatId,
            $"Спочатку увійди через {BotButtons.Login}",
            replyMarkup: GetStartKeyboard());
    }

    /// <summary>
    /// Keyboard shown before authentication.
    /// </summary>
    /// <returns>A <see cref="ReplyKeyboardMarkup"/> with login button.</returns>
    private static ReplyKeyboardMarkup GetStartKeyboard() => new(
    [
        [new KeyboardButton(BotButtons.Login)]
    ])
    {
        ResizeKeyboard = true,
    };

    /// <summary>
    /// Main keyboard shown after authentication.
    /// </summary>
    /// <returns>A <see cref="ReplyKeyboardMarkup"/> with main action buttons.</returns>
    private static ReplyKeyboardMarkup GetMainKeyboard() => new(
    [
        [KeyboardButton.WithRequestLocation(BotButtons.SendLocation)],
        [new KeyboardButton(BotButtons.StartTracking)],
        [new KeyboardButton(BotButtons.StartTrip)],
        [new KeyboardButton(BotButtons.EndTrip)]
    ])
    {
        ResizeKeyboard = true,
    };
}