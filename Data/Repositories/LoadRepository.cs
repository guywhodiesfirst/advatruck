namespace Data.Repositories;

using Core.Entities;
using Core.Enums;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class LoadRepository(TmsDataContext context) : ILoadRepository
{
    /// <inheritdoc />
    public async Task<Load?> GetByIdAsync(Guid loadId, CancellationToken cancellationToken = default)
        => await context.Loads
            .Include(l => l.Driver)
            .ThenInclude(d => d.User)
            .Include(l => l.Dispatcher)
            .ThenInclude(d => d.User)
            .Include(l => l.LoadStops)
            .FirstOrDefaultAsync(e => e.Id == loadId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Load>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Loads
            .Include(l => l.Driver)
            .ThenInclude(d => d.User)
            .Include(l => l.Dispatcher)
            .ThenInclude(d => d.User)
            .Include(l => l.LoadStops)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Load load, CancellationToken cancellationToken = default)
    {
        await context.Loads.AddAsync(load, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return load.Id;
    }

    /// <inheritdoc />
    public async Task<Load> UpdateAsync(Load load, CancellationToken cancellationToken = default)
    {
        context.Loads.Update(load);
        await context.SaveChangesAsync(cancellationToken);
        return load;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Load load, CancellationToken cancellationToken = default)
    {
        context.Loads.Remove(load);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Load?> GetNextActiveLoadByDriverIdAsync(Guid driverId, CancellationToken cancellationToken)
    {
        return await context.Loads
            .Include(l => l.LoadStops)
            .Include(l => l.Dispatcher).ThenInclude(d => d.User)
            .Where(l => l.DriverId == driverId &&
                        (l.LoadStatus == LoadStatus.Scheduled || l.LoadStatus == LoadStatus.Ongoing))
            .OrderBy(l => l.LoadStops.OrderBy(s => s.Timestamp).Select(s => s.Timestamp).FirstOrDefault())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Load>> GetAllByStatusAsync(LoadStatus status, CancellationToken cancellationToken = default)
        => await context.Loads
            .Include(l => l.Driver)
            .ThenInclude(d => d.User)
            .Include(l => l.Dispatcher)
            .ThenInclude(d => d.User)
            .Include(l => l.LoadStops)
            .Where(l => l.LoadStatus == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
}