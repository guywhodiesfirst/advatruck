using Core.Enums;

namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing loads in the database.
/// </summary>
public interface ILoadRepository
{
    /// <summary>
    /// Retrieve all the load records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the loads.</returns>
    Task<IEnumerable<Load>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the load by ID.
    /// </summary>
    /// <param name="loadId">Load ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Load.</returns>
    Task<Load?> GetByIdAsync(Guid loadId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a load to the database.
    /// </summary>
    /// <param name="load">Load.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created load.</returns>
    Task<Guid> AddAsync(Load load, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update load information.
    /// </summary>
    /// <param name="load">Load.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated load information.</returns>
    Task<Load> UpdateAsync(Load load, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the load from the database.
    /// </summary>
    /// <param name="load">Load.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Load load, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves next active load for the driver.
    /// </summary>
    /// <param name="driverId">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Next active load.</returns>
    Task<Load?> GetNextActiveLoadByDriverIdAsync(Guid driverId, CancellationToken cancellationToken  = default);

    /// <summary>
    /// Retrieves all the loads with the specified LoadStatus.
    /// </summary>
    /// <param name="status">Load status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Loads with the specified status.</returns>
    Task<IEnumerable<Load>> GetAllByStatusAsync(LoadStatus status, CancellationToken cancellationToken = default);
}