namespace Data.Repositories;

using Core.Entities;
using Core.Enums;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class AuctionLotRepository(TmsDataContext context) : IAuctionLotRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<AuctionLot>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        => await context.AuctionLots
            .Where(e => e.Status == AuctionStatus.Active && e.EndsAt > DateTime.UtcNow)
            .Include(e => e.DispatcherCreated).ThenInclude(d => d.User)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.User)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.DriverLocations)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<AuctionLot?> GetByIdAsync(Guid auctionLotId, CancellationToken cancellationToken = default)
        => await context.AuctionLots
            .Include(e => e.DispatcherCreated).ThenInclude(d => d.User)
            .Include(e => e.Load)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.User)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.DriverLocations)
            .FirstOrDefaultAsync(e => e.Id == auctionLotId, cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<AuctionLot>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.AuctionLots
            .Include(e => e.DispatcherCreated).ThenInclude(d => d.User)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.User)
            .Include(e => e.Bids).ThenInclude(b => b.DriverCreated).ThenInclude(d => d.DriverLocations)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Guid> AddAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default)
    {
        await context.AuctionLots.AddAsync(auctionLot, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return auctionLot.Id;
    }

    /// <inheritdoc />
    public async Task<AuctionLot> UpdateAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default)
    {
        context.AuctionLots.Update(auctionLot);
        await context.SaveChangesAsync(cancellationToken);
        return auctionLot;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(AuctionLot auctionLot, CancellationToken cancellationToken = default)
    {
        context.AuctionLots.Remove(auctionLot);
        await context.SaveChangesAsync(cancellationToken);
    }
}