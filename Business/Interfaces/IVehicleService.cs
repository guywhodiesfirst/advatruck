namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing vehicle business logic.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Retrieve all vehicles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of s.</returns>
    Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get vehicle by ID.
    /// </summary>
    /// <param name="id">Vehicle's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vehicle if found, otherwise throws an exception.</returns>
    Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new vehicle.
    /// </summary>
    /// <param name="vehicleDto">Vehicle to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created vehicle.</returns>
    Task<Guid> CreateAsync(VehicleCreateUpdateDto vehicleDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update vehicle information.
    /// </summary>
    /// <param name="vehicleDto">Vehicle with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated vehicle.</returns>
    Task<VehicleDto> UpdateAsync(VehicleCreateUpdateDto vehicleDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete vehicle by ID.
    /// </summary>
    /// <param name="id">Vehicle's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}