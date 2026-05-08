namespace Business.Interfaces;

using Core.Entities;
using Core.Models;

/// <summary>
/// Service for managing  business logic.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Retrieve all s.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of s.</returns>
    Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get  by ID.
    /// </summary>
    /// <param name="id">Vehicle's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vehicle if found, otherwise null.</returns>
    Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new .
    /// </summary>
    /// <param name="vehicleDto">Vehicle to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created .</returns>
    Task<Guid> CreateAsync(VehicleCreateUpdateDto vehicleDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update  information.
    /// </summary>
    /// <param name="vehicleDto">Vehicle with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated .</returns>
    Task<VehicleDto> UpdateAsync(VehicleCreateUpdateDto vehicleDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete  by ID.
    /// </summary>
    /// <param name="id">Vehicle's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}