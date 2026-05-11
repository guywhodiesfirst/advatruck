namespace Business.Interfaces;

using Core.Entities;
using Core.Models;

public interface IDriverLocationService
{
    /// <summary>
    /// Retrieves the latest known location for a driver.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// The most recent <see cref="DriverLocation"/> record.
    /// </returns>
    Task<DriverLocation?> GetLastAsync(Guid driverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new driver location tracking update.
    /// </summary>
    /// <param name="driverId">Driver ID.</param>
    /// <param name="request">The tracking update DTO.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The created <see cref="DriverLocation"/> entity.</returns>
    Task<DriverLocation> AddAsync(
        Guid driverId,
        TrackingUpdateRequestDto request,
        CancellationToken cancellationToken = default);
}