namespace Bot.Services;

using Bot.Interfaces;
using Bot.UI;
using Data.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

/// <inheritdoc/>
public class CommandService(
    ITelegramBotClient bot,
    IAuthService auth,
    IDriverSessionStore sessions)
    : ICommandService
{
    public async Task HandleAsync(long chatId, string text)
    {
        switch (text)
        {
            case "/start":
                await bot.SendMessage(
                    chatId,
                    "👋 Вітаю, водію!\nНатисни 'Увійти', щоб почати авторизацію",
                    replyMarkup: KeyboardLayout.StartKeyboard);
                return;

            case BotButtons.Login:
                if (await auth.IsAuthenticatedAsync(chatId))
                {
                    await bot.SendMessage(chatId, "✅ Ви вже в системі!", replyMarkup: KeyboardLayout.MainKeyboard);
                    return;
                }

                await sessions.MarkAwaitingEmailAsync(chatId);

                await bot.SendMessage(
                    chatId,
                    "Введи email:",
                    replyMarkup: new ReplyKeyboardRemove());
                return;

            case BotButtons.Logout:
                if (await auth.IsAuthenticatedAsync(chatId))
                {
                    await auth.HandleLogoutAsync(chatId);
                    await HandleAsync(chatId, "/start");
                    return;
                }

                await bot.SendMessage(chatId, "Ви не увійшли у систему!", replyMarkup: KeyboardLayout.StartKeyboard);
                return;

            case BotButtons.StartTrip:
                await bot.SendMessage(
                    chatId,
                    "🚀 Рейс розпочато",
                    replyMarkup: KeyboardLayout.MainKeyboard);
                return;

            case BotButtons.EndTrip:
                await bot.SendMessage(
                    chatId,
                    "✅ Рейс завершено",
                    replyMarkup: KeyboardLayout.MainKeyboard);
                return;

            case BotButtons.StartTracking:
                await bot.SendMessage(
                    chatId,
                    "📡 Щоб почати трекінг:\n\n" +
                      "1️⃣ Натисни '📎'\n" +
                      "2️⃣ Обери 'Місце'\n" +
                      "3️⃣ Натисни \"Поділитися моїм маячком на мапі\"\n\n" +
                      "Бот буде отримувати оновлення автоматично 🚀",
                    replyMarkup: KeyboardLayout.MainKeyboard);
                return;

            default:
                await bot.SendMessage(chatId, "Використай кнопки 👇", replyMarkup: KeyboardLayout.MainKeyboard);
                return;
        }
    }

    /// <inheritdoc/>
    public async Task SendLoginPromptAsync(long chatId, string text)
    {
        await sessions.MarkAwaitingEmailAsync(chatId);

        await bot.SendMessage(
            chatId,
            text,
            replyMarkup: new ReplyKeyboardRemove());
    }

    /// <inheritdoc/>
    public async Task SendPasswordPromptAsync(long chatId)
    {
        await bot.SendMessage(
            chatId,
            "🔑 Email прийнято. Тепер введи пароль:",
            replyMarkup: new ReplyKeyboardRemove());
    }

    public async Task SendAuthenticationFailedAsync(long chatId)
    {
        await bot.SendMessage(chatId, "🔐 Сесія вичерпана або ви не авторизовані.");
    }
}