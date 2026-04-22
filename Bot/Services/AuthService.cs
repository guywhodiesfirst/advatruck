namespace Bot.Services;

using Bot.Interfaces;
using Data.Interfaces;
using Telegram.Bot;

/// <inheritdoc/>
public class AuthService(
    ITelegramBotClient bot,
    DriverApiClient api,
    ICommandService commands,
    IDriverSessionStore sessions) : IAuthService
{
    /// <inheritdoc/>
    public async Task HandleLoginAsync(long chatId, string email)
    {
        var driver = await api.LoginAsync(email);

        if (driver == null)
        {
            await bot.SendMessage(chatId, "❌ Не знайдено користувача");
            return;
        }

        await sessions.SaveAuthenticatedDriverAsync(chatId, driver.Id);

        await commands.SendGreetingAsync(chatId, driver.FirstName);
    }
}