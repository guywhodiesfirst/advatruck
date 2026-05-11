namespace Business.Interfaces;

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
    /// Get auction lot by ID.
    /// </summary>
    /// <param name="id">AuctionLot's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>AuctionLot if found, otherwise null.</returns>
    Task<AuctionLotDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new auction lot.
    /// </summary>
    /// <param name="dto">AuctionLot to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created auction lot.</returns>
    Task<Guid> CreateAsync(AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update auction lot information.
    /// </summary>
    /// <param name="dto">AuctionLot with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated auction lot.</returns>
    Task<AuctionLotDto> UpdateAsync(AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete auction lot by ID.
    /// </summary>
    /// <param name="id">AuctionLot's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}