using Data.Models;

namespace Data.Interfaces;

public interface IDriverLocationRepository
{
    /// <summary>
    /// Retrieve a history of driver's locations by driver ID.
    /// </summary>
    /// <param name="driverId">Driver ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of locations.</returns>
    Task<IEnumerable<DriverLocation>> GetAllByDriverIdAsync(Guid driverId,
        CancellationToken cancellationToken = default);  
    
    /// <summary>
    /// Get driver's most recent location by driver ID.
    /// </summary>
    /// <param name="driverId">Driver ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver location with the most recent update time.</returns>
    Task<DriverLocation?> GetLastByDriverIdAsync(Guid driverId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add new driver location to the database.
    /// </summary>
    /// <param name="driverLocation">Driver location.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<DriverLocation> AddAsync(DriverLocation driverLocation,
        CancellationToken cancellationToken = default);
}