namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing  business logic.
/// </summary>
public interface ILoadService
{
    /// <summary>
    /// Retrieve all s.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of loads.</returns>
    Task<IEnumerable<LoadDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get  by ID.
    /// </summary>
    /// <param name="id">Load's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Load if found, otherwise throws an exception.</returns>
    Task<LoadDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new .
    /// </summary>
    /// <param name="dto">Load to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created load.</returns>
    Task<Guid> CreateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update  information.
    /// </summary>
    /// <param name="dto">Load with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load.</returns>
    Task<LoadDto> UpdateAsync(LoadCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete  by ID.
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
}