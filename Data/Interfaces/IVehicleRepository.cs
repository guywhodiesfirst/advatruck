namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing vehicles in the database.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Retrieve all the vehicle records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the vehicles.</returns>
    Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the vehicle by ID.
    /// </summary>
    /// <param name="vehicleId">Vehicle ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vehicle.</returns>
    Task<Vehicle?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a vehicle to the database.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created vehicle.</returns>
    Task<Guid> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update vehicle information.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated vehicle information.</returns>
    Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the vehicle from the database.
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
}