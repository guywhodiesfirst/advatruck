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
                    "📧 Введи email:",
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

            case BotButtons.StartTracking:
                await bot.SendMessage(
                    chatId,
                    "📡 Щоб почати трекінг:\n\n" +
                      "1️⃣ Натисни '📎'\n" +
                      "2️⃣ Обери 'Місце'\n" +
                      "3️⃣ Натисни 'Поділитися моїм маячком у реальному часі'\n\n" +
                      "Бот отримуватиме геопозицію автоматично 🛰",
                    replyMarkup: KeyboardLayout.MainKeyboard);
                return;

            default:
                var isAuthenticated = await auth.IsAuthenticatedAsync(chatId);
                var keyboard = isAuthenticated ? KeyboardLayout.MainKeyboard : KeyboardLayout.StartKeyboard;

                await bot.SendMessage(chatId, "Скористайся кнопками меню 👇", replyMarkup: keyboard);
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

    /// <inheritdoc/>
    public async Task SendAuthenticationFailedAsync(long chatId)
    {
        await bot.SendMessage(
            chatId,
            "🔐 Сесія вичерпана або ви не авторизовані.",
            replyMarkup: KeyboardLayout.StartKeyboard);
    }

    /// <inheritdoc/>
    public async Task HandleErrorAsync(long chatId, string message)
    {
        await sessions.ClearAwaitingEmailAsync(chatId);
        await sessions.ClearAwaitingPasswordAsync(chatId);

        var isAuthenticated = await auth.IsAuthenticatedAsync(chatId);
        var keyboard = isAuthenticated ? KeyboardLayout.MainKeyboard : KeyboardLayout.StartKeyboard;

        await bot.SendMessage(
            chatId,
            $"❌ {message}",
            replyMarkup: keyboard);
    }
}