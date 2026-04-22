namespace Bot.Interfaces;

/// <summary>
/// Handles driver authentication flow in Telegram bot.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Processes the login flow for a driver based on the provided email.
    /// </summary>
    /// <param name="chatId">Telegram chat ID of the driver.</param>
    /// <param name="email">The email address of the driver.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleLoginAsync(long chatId, string email);
}