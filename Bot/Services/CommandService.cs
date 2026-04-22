namespace Bot.Services;

using Bot.Interfaces;
using Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

/// <inheritdoc/>
public class CommandService(
    ITelegramBotClient bot,
    IDriverSessionStore sessions)
    : ICommandService
{
    public async Task HandleAsync(long chatId, string text)
    {
        switch (text)
        {
            case "/start":
                await bot.SendMessage(chatId, "👋 Старт");
                return;

            case BotButtons.Login:
                await sessions.MarkAwaitingEmailAsync(chatId);
                await bot.SendMessage(
                    chatId,
                    "Введи email:",
                    replyMarkup: GetStartKeyboard());
                return;

            case BotButtons.StartTrip:
                await bot.SendMessage(
                    chatId,
                    "🚀 Рейс розпочато",
                    replyMarkup: GetMainKeyboard());
                return;

            case BotButtons.EndTrip:
                await sessions.ClearAuthenticatedDriverAsync(chatId);
                await sessions.ClearTrackingAsync(chatId);
                await bot.SendMessage(
                    chatId,
                    "✅ Рейс завершено",
                    replyMarkup: GetMainKeyboard());
                return;

            case BotButtons.StartTracking:
                await bot.SendMessage(
                    chatId,
                    "📡 Щоб почати трекінг:\n\n" +
                      "1️⃣ Натисни '📎'\n" +
                      "2️⃣ Обери 'Місце'\n" +
                      "3️⃣ Натисни \"Поділитися моїм маячком на мапі\"\n\n" +
                      "Бот буде отримувати оновлення автоматично 🚀",
                    replyMarkup: GetMainKeyboard());
                return;

            default:
                await bot.SendMessage(chatId, "Використай кнопки 👇");
                return;
        }
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

    /// <inheritdoc/>
    public async Task SendLoginPromptAsync(long chatId, string text)
    {
        await sessions.MarkAwaitingEmailAsync(chatId);

        await bot.SendMessage(
            chatId,
            text,
            replyMarkup: GetStartKeyboard());
    }

    /// <inheritdoc/>
    public async Task SendGreetingAsync(long chatId, string name)
    {
        await bot.SendMessage(
            chatId,
            $"✅ Вітаю, {name}",
            replyMarkup: GetMainKeyboard());
    }

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