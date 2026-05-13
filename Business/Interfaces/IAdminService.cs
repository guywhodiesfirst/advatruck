namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing admin business logic.
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// Retrieve all admins.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of admins.</returns>
    Task<IEnumerable<AdminDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get admin by ID.
    /// </summary>
    /// <param name="id">Admin's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Admin if found, otherwise null.</returns>
    Task<AdminDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new admin.
    /// </summary>
    /// <param name="dto">Admin to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created admin.</returns>
    Task<Guid> CreateAsync(AdminCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update admin information.
    /// </summary>
    /// <param name="dto">Admin with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated admin.</returns>
    Task<AdminDto> UpdateAsync(AdminCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete admin by ID.
    /// </summary>
    /// <param name="id">Admin's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}