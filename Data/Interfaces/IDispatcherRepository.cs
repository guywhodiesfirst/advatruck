namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing dispatchers in the database.
/// </summary>
public interface IDispatcherRepository
{
    /// <summary>
    /// Retrieve all the dispatcher records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the dispatchers.</returns>
    Task<IEnumerable<Dispatcher>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the dispatcher by ID.
    /// </summary>
    /// <param name="dispatcherId">Dispatcher ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dispatcher.</returns>
    Task<Dispatcher?> GetByIdAsync(Guid dispatcherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the dispatcher by email.
    /// </summary>
    /// <param name="email">Dispatcher's email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dispatcher.</returns>
    Task<Dispatcher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a dispatcher to the database.
    /// </summary>
    /// <param name="dispatcher">Dispatcher.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created dispatcher.</returns>
    Task<Guid> AddAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update dispatcher information.
    /// </summary>
    /// <param name="dispatcher">Dispatcher.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated dispatcher information.</returns>
    Task<Dispatcher> UpdateAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the dispatcher from the database.
    /// </summary>
    /// <param name="dispatcher">Dispatcher.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default);
}