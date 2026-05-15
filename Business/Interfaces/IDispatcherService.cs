namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing dispatcher business logic.
/// </summary>
public interface IDispatcherService
{
    /// <summary>
    /// Retrieve all dispatchers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of s.</returns>
    Task<IEnumerable<DispatcherDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get dispatcher by ID.
    /// </summary>
    /// <param name="id">Dispatcher's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dispatcher if found, otherwise null.</returns>
    Task<DispatcherDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new dispatcher.
    /// </summary>
    /// <param name="dto">Dispatcher to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created .</returns>
    Task<Guid> CreateAsync(DispatcherCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update dispatcher information.
    /// </summary>
    /// <param name="dto">Dispatcher with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated dispatcher.</returns>
    Task<DispatcherDto> UpdateAsync(DispatcherCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete dispatcher by ID.
    /// </summary>
    /// <param name="id">Dispatcher's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full dispatcher profile information by email.
    /// </summary>
    /// <param name="email">Dispatcher's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dispatcher profile data transfer object.</returns>
    Task<DispatcherProfileDto?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full dispatcher profile information by ID.
    /// </summary>
    /// <param name="id">Dispatcher's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dispatcher profile data transfer object.</returns>
    Task<DispatcherProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
}