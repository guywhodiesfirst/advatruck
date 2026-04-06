using Core.Entities;
using Telegram.Bot.Types;

namespace Bot.Interfaces;

/// <summary>
/// Client for interaction with Driver API.
/// </summary>
public interface IDriverApiClient
{
    /// <summary>
    /// Authenticates a driver by email.
    /// </summary>
    /// <param name="email">Driver's email address.</param>
    /// <returns>
    /// Driver entity if authentication is successful; otherwise, null.
    /// </returns>
    Task<Driver?> LoginAsync(string email);

    /// <summary>
    /// Sends the current location of a driver to the API.
    /// </summary>
    /// <param name="driverId">Unique identifier of the driver.</param>
    /// <param name="location">Location data received from Telegram.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendLocationAsync(Guid driverId, Location location);
}