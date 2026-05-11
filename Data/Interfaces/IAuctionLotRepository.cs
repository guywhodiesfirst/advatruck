namespace Data.Interfaces;

using Core.Entities;

/// <summary>
/// Repository for managing auctionLots in the database.
/// </summary>
public interface IAuctionLotRepository
{
    /// <summary>
    /// Retrieve all the auctionLot records from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List containing all the auctionLots.</returns>
    Task<IEnumerable<AuctionLot>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the auctionLot by ID.
    /// </summary>
    /// <param name="auctionLotId">AuctionLot ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>AuctionLot.</returns>
    Task<AuctionLot?> GetByIdAsync(Guid auctionLotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a auctionLot to the database.
    /// </summary>
    /// <param name="auctionLot">AuctionLot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of created auctionLot.</returns>
    Task<Guid> AddAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update auctionLot information.
    /// </summary>
    /// <param name="auctionLot">AuctionLot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated auctionLot information.</returns>
    Task<AuctionLot> UpdateAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the auctionLot from the database.
    /// </summary>
    /// <param name="auctionLot">AuctionLot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default);
}