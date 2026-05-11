namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing bids in the database.
/// </summary>
public interface IBidRepository
{
    /// <summary>
    /// Retrieve all the bid records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the bids.</returns>
    Task<IEnumerable<Bid>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the bid by ID.
    /// </summary>
    /// <param name="bidId">Bid ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bid.</returns>
    Task<Bid?> GetByIdAsync(Guid bidId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a bid to the database.
    /// </summary>
    /// <param name="bid">Bid.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created bid.</returns>
    Task<Guid> AddAsync(Bid bid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update bid information.
    /// </summary>
    /// <param name="bid">Bid.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated bid information.</returns>
    Task<Bid> UpdateAsync(Bid bid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the bid from the database.
    /// </summary>
    /// <param name="bid">Bid.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Bid bid, CancellationToken cancellationToken = default);
}