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
    /// Delete bid by ID.
    /// </summary>
    /// <param name="id">Bid's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upsert bid.
    /// </summary>
    /// <param name="driverId">Driver ID.</param>
    /// <param name="dto">DTO.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bid ID.</returns>
    Task<Guid> UpsertAsync(Guid driverId, BidCreateUpdateDto dto, CancellationToken cancellationToken = default);
}