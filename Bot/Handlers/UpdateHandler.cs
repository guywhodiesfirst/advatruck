namespace Bot.Handlers;

using Bot.Interfaces;
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

        if (await sessions.IsAwaitingEmailAsync(chatId))
        {
            await auth.HandleLoginAsync(chatId, text);
            await sessions.ClearAwaitingEmailAsync(chatId);
            return;
        }

        await commands.HandleAsync(chatId, text);
    }
}