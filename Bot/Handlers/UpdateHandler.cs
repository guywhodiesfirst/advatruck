namespace Bot.Handlers;

using Bot.Interfaces;
using Bot.UI;
using Data.Interfaces;
using Telegram.Bot.Types;

/// <summary>
/// Handles incoming Telegram updates and routes them to appropriate handlers.
/// </summary>
public class UpdateHandler(
    ITrackingService tracking,
    IAuthService auth,
    ICommandService commands,
    IDriverSessionStore sessions)
{
    public async Task HandleAsync(Update update)
    {
        var message = update.Message ?? update.EditedMessage;
        if (message == null)
        {
            return;
        }

        var chatId = message.Chat.Id;

        if (message.Location != null)
        {
            await tracking.HandleLocationAsync(chatId, message.Location);
            return;
        }

        if (message.Text is not { } text)
        {
            return;
        }

        if (text is "/start" or BotButtons.Login)
        {
            await sessions.ClearAwaitingEmailAsync(chatId);
            await sessions.ClearAwaitingPasswordAsync(chatId);
        }

        if (await sessions.IsAwaitingEmailAsync(chatId))
        {
            await sessions.ClearAwaitingEmailAsync(chatId);
            await sessions.MarkAwaitingPasswordAsync(chatId, text);
            await commands.SendPasswordPromptAsync(chatId);
            return;
        }

        var pwState = await sessions.GetAwaitingPasswordStateAsync(chatId);
        if (pwState.IsAwaiting && !string.IsNullOrEmpty(pwState.Email))
        {
            var actualState = await sessions.GetAwaitingPasswordStateAsync(chatId);
            if (actualState.IsAwaiting)
            {
                await auth.HandleLoginAsync(chatId, pwState.Email, text);
                return;
            }
        }

        if (text != "/start" && text != BotButtons.Login)
        {
            var isAuthenticated = await auth.IsAuthenticatedAsync(chatId);
            if (!isAuthenticated)
            {
                await sessions.ClearSessionAsync(chatId);
                await commands.SendAuthenticationFailedAsync(chatId);
                await commands.HandleAsync(chatId, "/start");
                return;
            }
        }

        await commands.HandleAsync(chatId, text);
    }
}