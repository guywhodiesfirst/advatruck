namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing bid business logic.
/// </summary>
public interface IBidService
{
    /// <summary>
    /// Retrieve all bids.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of bids.</returns>
    Task<IEnumerable<BidDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get bid by ID.
    /// </summary>
    /// <param name="id">Bid's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bid if found, otherwise null.</returns>
    Task<BidDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new bid.
    /// </summary>
    /// <param name="dto">Bid to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created bid.</returns>
    Task<Guid> CreateAsync(BidCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update bid information.
    /// </summary>
    /// <param name="currentUserId">
    /// ID of the driver who updates the bid.
    /// Prevents driver from updating bids other than theirs.
    /// </param>
    /// <param name="dto">Bid with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated bid.</returns>
    Task<BidDto> UpdateAsync(Guid currentUserId, BidCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete bid by ID.
    /// </summary>
    /// <param name="id">Bid's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}