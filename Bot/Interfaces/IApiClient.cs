namespace Bot.Interfaces;

using Core.Models;
using Telegram.Bot.Types;

/// <summary>
/// Client for interaction with Driver API.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Authenticates a driver by email.
    /// </summary>
    /// <param name="email">Driver's email address.</param>
    /// <param name="password">Driver's password.</param>
    /// <returns>
    /// Driver entity if authentication is successful; otherwise, null.
    /// </returns>
    Task<AuthResponseDto?> LoginAsync(string email, string password);

    /// <summary>
    /// Sends the current location of a driver to the API.
    /// </summary>
    /// <param name="driverId">Unique identifier of the driver.</param>
    /// <param name="location">Location data received from Telegram.</param>
    /// <param name="token">JWT token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendLocationAsync(Guid driverId, Location location, string token);

    /// <summary>
    /// Retrieves full driver information.
    /// </summary>
    /// <param name="token">JWT token.</param>
    /// <returns>Driver profile.</returns>
    Task<DriverProfileDto?> GetProfileAsync(string token);

    /// <summary>
    /// Retrieves load with specified ID.
    /// </summary>
    /// <param name="loadId">Load ID.</param>
    /// <param name="token">JWT token.</param>
    /// <returns>Active load.</returns>
    Task<LoadDto?> GetLoadByIdAsync(Guid loadId, string token);
}