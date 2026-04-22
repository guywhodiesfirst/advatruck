namespace Bot.Interfaces;

using Telegram.Bot.Types;

/// <summary>
/// Handles driver tracking logic (throttling, live tracking updates, persistence).
/// </summary>
public interface ITrackingService
{
    /// <summary>
    /// Processes a location update for a driver.
    /// </summary>
    /// <param name="chatId">Telegram chat ID of the driver.</param>
    /// <param name="location">The location update.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleLocationAsync(long chatId, Location location);

    /// <summary>
    /// Stops tracking for a driver.
    /// </summary>
    /// <param name="chatId">Telegram chat ID of the driver.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StopTrackingAsync(long chatId);
}