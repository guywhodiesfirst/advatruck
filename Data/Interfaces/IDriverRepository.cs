using Data.Models;

namespace Data.Interfaces;

/// <summary>
/// Repository for managing drivers in the database.
/// </summary>
public interface IDriverRepository
{
    /// <summary>
    /// Retrieve all the driver records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the drivers.</returns>
    Task<IEnumerable<Driver>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get the driver by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver.</returns>
    Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add a driver to the database.
    /// </summary>
    /// <param name="driver">Driver.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created driver.</returns>
    Task<Guid> AddAsync(Driver driver,  CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove the driver from the database.
    /// </summary>
    /// <param name="driver">Driver.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Driver driver,   CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update driver information.
    /// </summary>
    /// <param name="driver">Driver.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated driver information.</returns>
    Task<Driver> UpdateAsync(Driver driver,  CancellationToken cancellationToken = default);
}