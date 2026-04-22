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
    /// Typically used when the user attempts to access functionality without being authenticated.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="text">Optional message text to display alongside the login prompt.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendLoginPromptAsync(long chatId, string text);

    /// <summary>
    /// Greets a driver after login.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="name">Driver's name.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendGreetingAsync(long chatId, string name);
}