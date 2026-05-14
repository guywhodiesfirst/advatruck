namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing load business logic.
/// </summary>
public interface ILoadService
{
    /// <summary>
    /// Retrieve all loads.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of loads.</returns>
    Task<IEnumerable<LoadDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get load by ID.
    /// </summary>
    /// <param name="id">Load's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Load if found, otherwise throws an exception.</returns>
    Task<LoadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new load.
    /// </summary>
    /// <param name="dto">Load to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created load.</returns>
    Task<Guid> CreateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update load information.
    /// </summary>
    /// <param name="dto">Load with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load.</returns>
    Task<LoadDto> UpdateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete load by ID.
    /// </summary>
    /// <param name="id">Load's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update only the status of the load.
    /// </summary>
    /// <param name="dto">Status update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load data.</returns>
    Task<LoadDto> UpdateStatusAsync(LoadStatusUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign driver to the load.
    /// </summary>
    /// <param name="dto">DTO.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load data.</returns>
    Task<LoadDto> AssignDriverAsync(LoadAssignDriverDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove driver from the load.
    /// </summary>
    /// <param name="id">Load ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load data.</returns>
    Task<LoadDto> DeassignDriverAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves next active load assigned to the driver.
    /// </summary>
    /// <param name="driverId">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Next active load if exists, otherwise null.</returns>
    Task<LoadDto?> GetActiveLoadByIdAsync(Guid driverId, CancellationToken cancellationToken = default);
}