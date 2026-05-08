namespace Bot.Services;

using Bot.Interfaces;
using Bot.UI;
using Data.Interfaces;
using Telegram.Bot;

/// <inheritdoc/>
public class AuthService(
    ITelegramBotClient bot,
    IApiClient api,
    IDriverSessionStore sessions) : IAuthService
{
    /// <inheritdoc/>
    public async Task HandleLoginAsync(long chatId, string email, string password)
    {
        await sessions.ClearSessionAsync(chatId);
        var authData = await api.LoginAsync(email, password);

        if (authData == null)
        {
            await sessions.ClearAwaitingPasswordAsync(chatId);
            await sessions.ClearAwaitingEmailAsync(chatId);

            await bot.SendMessage(
                chatId,
                "❌ Помилка авторизації. Можливо, email або пароль невірні.\n" +
                "Натисніть кнопку 'Увійти', щоб спробувати ще раз.",
                replyMarkup: KeyboardLayout.StartKeyboard);
            return;
        }

        await sessions.SaveBindingAsync(chatId, authData.Id, authData.Token);

        await sessions.ClearAwaitingPasswordAsync(chatId);

        var profile = await api.GetProfileAsync(authData.Token);

        await SendGreetingAsync(chatId, profile!.FirstName);
    }

    /// <inheritdoc/>
    public async Task HandleLogoutAsync(long chatId)
    {
        await sessions.ClearSessionAsync(chatId);

        await bot.SendMessage(
            chatId,
            "🚪 Ви вийшли з системи. До зустрічі!",
            replyMarkup: new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove());
    }

    /// <inheritdoc/>
    public async Task<bool> IsAuthenticatedAsync(long chatId)
    {
        var driverId = await sessions.GetDriverIdByChatIdAsync(chatId);
        if (!driverId.HasValue)
        {
            return false;
        }

        var token = await sessions.GetTokenByDriverIdAsync(driverId.Value);
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            var profile = await api.GetProfileAsync(token);
            return profile != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task SendGreetingAsync(long chatId, string name)
    {
        await bot.SendMessage(
            chatId,
            $"✅ Вітаю, {name}",
            replyMarkup: KeyboardLayout.MainKeyboard);
    }
}