namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing admins in the database.
/// </summary>
public interface IAdminRepository
{
    /// <summary>
    /// Retrieve all the admin records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the admins.</returns>
    Task<IEnumerable<Admin>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the admin by ID.
    /// </summary>
    /// <param name="adminId">Admin ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Admin.</returns>
    Task<Admin?> GetByIdAsync(Guid adminId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the admin by email.
    /// </summary>
    /// <param name="email">Admin's email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Admin.</returns>
    Task<Admin?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a admin to the database.
    /// </summary>
    /// <param name="admin">Admin.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created admin.</returns>
    Task<Guid> AddAsync(Admin admin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update admin information.
    /// </summary>
    /// <param name="admin">Admin.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated admin information.</returns>
    Task<Admin> UpdateAsync(Admin admin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the admin from the database.
    /// </summary>
    /// <param name="admin">Admin.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Admin admin, CancellationToken cancellationToken = default);
}