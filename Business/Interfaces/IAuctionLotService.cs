namespace Business.Interfaces;

using Core.Enums;
using Core.Models;

/// <summary>
/// Service for managing auction lot business logic.
/// </summary>
public interface IAuctionLotService
{
    /// <summary>
    /// Retrieve all auction lots.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of auction lots.</returns>
    Task<IEnumerable<AuctionLotDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve all active auction lots.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active auction lots.</returns>
    Task<IEnumerable<AuctionLotDto>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get auction lot by ID.
    /// </summary>
    /// <param name="id">AuctionLot's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>AuctionLot if found, otherwise null.</returns>
    Task<AuctionLotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new auction lot.
    /// </summary>
    /// <param name="dto">Auction lot to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created auction lot.</returns>
    Task<Guid> CreateAsync(AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update auction lot information.
    /// </summary>
    /// <param name="id">Auction lot ID.</param>
    /// <param name="dto">Auction lot with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated auction lot.</returns>
    Task<AuctionLotDto> UpdateAsync(Guid id, AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete auction lot by ID.
    /// </summary>
    /// <param name="id">Auction lot's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates only the status of the auction lot.
    /// </summary>
    /// <param name="id">Auction lot ID.</param>
    /// <param name="status">New auction status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task<AuctionLotDto> UpdateStatusAsync(Guid id, AuctionStatus status, CancellationToken cancellationToken);
}