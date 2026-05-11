namespace Bot.Interfaces;

/// <summary>
/// Handles bot command routing and responses.
/// </summary>
public interface ICommandService
{
    /// <summary>
    /// Processes an incoming text message and executes the corresponding command logic.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="text">The text message received.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(long chatId, string text);

    /// <summary>
    /// Sends a login prompt message to the user and displays the keyboard with the login action.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="text">Optional message text to display alongside the login prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendLoginPromptAsync(long chatId, string text);

    /// <summary>
    /// Sends a password input message to the user.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendPasswordPromptAsync(long chatId);

    /// <summary>
    /// Sends a message notifying user about failed authentication.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendAuthenticationFailedAsync(long chatId);

    /// <summary>
    /// Handles errors and provides user with error message.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="message">Error message.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleErrorAsync(long chatId, string message);
}