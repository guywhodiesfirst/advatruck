using Core.Entities;

namespace Business.Interfaces;

/// <summary>
/// Service for managing driver business logic.
/// </summary>
public interface IDriverService
{
    /// <summary>
    /// Retrieve all drivers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of drivers.</returns>
    Task<IEnumerable<Driver>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get driver by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver if found, otherwise null.</returns>
    Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Login driver by email.
    /// </summary>
    /// <param name="email">Driver's email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver if found, otherwise null.</returns>
    Task<Driver?> LoginAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new driver.
    /// </summary>
    /// <param name="driver">Driver to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created driver.</returns>
    Task<Guid> CreateAsync(Driver driver, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete driver by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update driver information.
    /// </summary>
    /// <param name="driver">Driver with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated driver.</returns>
    Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default);
}