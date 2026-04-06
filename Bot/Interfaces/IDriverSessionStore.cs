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
    Task SaveAuthenticatedDriverAsync(long chatId, Guid driverId);

    /// <summary>
    /// Retrieves authenticated driver session for the specified chat.
    /// </summary>
    /// <returns>Driver ID if session exists; otherwise null.</returns>
    Task<Guid?> GetAuthenticatedDriverAsync(long chatId);

    /// <summary>
    /// Removes authenticated driver session for the specified chat.
    /// </summary>
    Task ClearAuthenticatedDriverAsync(long chatId);

    /// <summary>
    /// Marks that the user has started login process and is expected to send email.
    /// </summary>
    Task MarkAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Checks whether the user is currently in the login (awaiting email) state.
    /// </summary>
    Task<bool> IsAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Removes the login state after email has been processed.
    /// </summary>
    Task ClearAwaitingEmailAsync(long chatId);
    
    /// <summary>
    /// Saves the message ID of the tracking status message for a specific chat.
    /// This message will be updated instead of sending new messages on each location update.
    /// </summary>
    Task SaveTrackingMessageIdAsync(long chatId, int messageId);

    /// <summary>
    /// Retrieves the message ID of the tracking status message for a specific chat.
    /// </summary>
    /// <returns>The message ID if exists; otherwise null.</returns>
    Task<int?> GetTrackingMessageIdAsync(long chatId);
    
    /// <summary>
    /// Marks that live tracking is currently active.
    /// </summary>
    Task SetTrackingActiveAsync(long chatId);

    /// <summary>
    /// Checks whether live tracking is active.
    /// </summary>
    Task<bool> IsTrackingActiveAsync(long chatId);

    /// <summary>
    /// Clears tracking state.
    /// </summary>
    Task ClearTrackingAsync(long chatId);
}