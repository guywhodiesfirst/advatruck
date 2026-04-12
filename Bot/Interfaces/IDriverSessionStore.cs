namespace Bot.Interfaces;

/// <summary>
/// Provides access to driver session storage.
/// Handles both authentication state and login flow state
/// associated with a Telegram chat.
/// </summary>
public interface IDriverSessionStore
{
    /// <summary>
    /// Stores authenticated driver session for the specified chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="driverId">Authenticated driver ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveAuthenticatedDriverAsync(long chatId, Guid driverId);

    /// <summary>
    /// Retrieves authenticated driver session for the specified chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>Driver ID if session exists; otherwise null.</returns>
    Task<Guid?> GetAuthenticatedDriverAsync(long chatId);

    /// <summary>
    /// Removes authenticated driver session for the specified chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearAuthenticatedDriverAsync(long chatId);

    /// <summary>
    /// Marks that the user has started login process and is expected to send email.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task MarkAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Checks whether the user is currently in the login (awaiting email) state.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> IsAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Removes the login state after email has been processed.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Saves the message ID of the tracking status message for a specific chat.
    /// This message will be updated instead of sending new messages on each location update.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="messageId">Telegram message ID to save.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveTrackingMessageIdAsync(long chatId, int messageId);

    /// <summary>
    /// Retrieves the message ID of the tracking status message for a specific chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>The message ID if exists; otherwise null.</returns>
    Task<int?> GetTrackingMessageIdAsync(long chatId);

    /// <summary>
    /// Marks that live tracking is currently active.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetTrackingActiveAsync(long chatId);

    /// <summary>
    /// Checks whether live tracking is active.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<bool> IsTrackingActiveAsync(long chatId);

    /// <summary>
    /// Clears tracking state.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearTrackingAsync(long chatId);
}