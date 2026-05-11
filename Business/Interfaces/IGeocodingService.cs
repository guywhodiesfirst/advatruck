namespace Business.Interfaces;

using Core.Types;

/// <summary>
/// Provides methods for converting geographic coordinates into human-readable addresses.
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Retrieves an address for specified coordinates.
    /// </summary>
    /// <param name="location">Location.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Address.</returns>
    Task<string> GetAddressAsync(GeoPoint location, CancellationToken cancellationToken = default);
}