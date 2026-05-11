namespace Data.Interfaces;

using Core.Models;

/// <summary>
/// Provides access to driver session storage.
/// </summary>
public interface IDriverSessionStore
{
    /// <summary>
    /// Creates a bidirectional binding between Telegram chat, driver ID and their JWT token.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="driverId">Authenticated driver ID.</param>
    /// <param name="token">JWT access token for API calls.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveBindingAsync(long chatId, Guid driverId, string token);

    /// <summary>
    /// Retrieves authenticated driver ID for the specified chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>Driver ID if session exists; otherwise null.</returns>
    Task<Guid?> GetDriverIdByChatIdAsync(long chatId);

    /// <summary>
    /// Retrieves the JWT access token for a specific driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>JWT token if exists; otherwise null.</returns>
    Task<string?> GetTokenByDriverIdAsync(Guid driverId);

    /// <summary>
    /// Retrieves Telegram chatId for a given driverId.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>ChatId.</returns>
    Task<long?> GetChatIdByDriverIdAsync(Guid driverId);

    /// <summary>
    /// Removes all session data (token, mappings, states) associated with the chat.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearSessionAsync(long chatId);

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
    /// <returns>A bool indicating the state.</returns>
    Task<bool> IsAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Removes the login state after email has been processed.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearAwaitingEmailAsync(long chatId);

    /// <summary>
    /// Marks that live tracking is currently active for the driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetTrackingActiveAsync(Guid driverId);

    /// <summary>
    /// Checks whether live tracking is active for the driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>A bool indicating the state.</returns>
    Task<bool> IsTrackingActiveAsync(Guid driverId);

    /// <summary>
    /// Saves the message ID of the tracking status message for a specific driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <param name="messageId">Telegram message ID to save.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveTrackingMessageIdAsync(Guid driverId, int messageId);

    /// <summary>
    /// Retrieves the message ID of the tracking status message for a specific driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>The message ID if exists; otherwise null.</returns>
    Task<int?> GetTrackingMessageIdAsync(Guid driverId);

    /// <summary>
    /// Clears tracking state and message ID for the driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearTrackingAsync(Guid driverId);

    /// <summary>
    /// Saves timestamp of the last received location update for a driver.
    /// </summary>
    /// <param name="driverId">ID of the driver.</param>
    /// <param name="time">UTC timestamp of the last location update.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveLastLocationUpdateAsync(Guid driverId, DateTime time);

    /// <summary>
    /// Retrieves timestamp of the last received location update for a driver.
    /// </summary>
    /// <param name="driverId">ID of the driver.</param>
    /// <returns>The last update timestamp if exists; otherwise null.</returns>
    Task<DateTime?> GetLastLocationUpdateAsync(Guid driverId);

    /// <summary>
    /// Saves timestamp when tracking was stopped for a specific driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <param name="time">UTC timestamp when tracking was stopped.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveTrackingStoppedAtAsync(Guid driverId, DateTime time);

    /// <summary>
    /// Retrieves timestamp when tracking was last stopped for a specific driver.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>Timestamp of the last time when tracking stopped.</returns>
    Task<DateTime?> GetTrackingStoppedAtAsync(Guid driverId);

    /// <summary>
    /// Clears driver TrackingStoppedAt timestamp.
    /// </summary>
    /// <param name="driverId">Driver identifier.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearTrackingStoppedAtAsync(Guid driverId);

    /// <summary>
    /// Marks that the user is expected to send password.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <param name="email">Driver's email.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task MarkAwaitingPasswordAsync(long chatId, string email);

    /// <summary>
    /// Retrieves password await state.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>Password await state.</returns>
    Task<PasswordAwaitState> GetAwaitingPasswordStateAsync(long chatId);

    /// <summary>
    /// Clears awaiting password.
    /// </summary>
    /// <param name="chatId">Telegram chat ID.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ClearAwaitingPasswordAsync(long chatId);
}