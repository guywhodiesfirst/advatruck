namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class BidRepository(TmsDataContext context) : IBidRepository
{
    /// <inheritdoc />
    public async Task<Bid?> GetByIdAsync(Guid bidId, CancellationToken cancellationToken = default)
        => await context.Bids
            .Include(e => e.DriverCreated)
                .ThenInclude(d => d.User)
            .Include(b => b.DriverCreated)
                .ThenInclude(d => d.DriverLocations)
            .Include(b => b.AuctionLot)
                .ThenInclude(al => al.Load)
                    .ThenInclude(l => l.LoadStops)
            .FirstOrDefaultAsync(e => e.Id == bidId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Bid>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Bids
            .Include(b => b.DriverCreated)
                .ThenInclude(d => d.User)
            .Include(b => b.DriverCreated)
                .ThenInclude(d => d.DriverLocations)
            .Include(b => b.AuctionLot)
                .ThenInclude(al => al.Load)
                    .ThenInclude(l => l.LoadStops)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        await context.Bids.AddAsync(bid, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return bid.Id;
    }

    /// <inheritdoc />
    public async Task<Bid> UpdateAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        context.Bids.Update(bid);
        await context.SaveChangesAsync(cancellationToken);
        return bid;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Bid bid, CancellationToken cancellationToken = default)
    {
        context.Bids.Remove(bid);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Bid?> GetByDriverAndLotAsync(Guid driverId, Guid auctionLotId, CancellationToken cancellationToken = default)
    {
        return await context.Bids
            .Include(b => b.AuctionLot)
            .FirstOrDefaultAsync(b => b.DriverCreatedId == driverId && b.AuctionLotId == auctionLotId, cancellationToken);
    }
}