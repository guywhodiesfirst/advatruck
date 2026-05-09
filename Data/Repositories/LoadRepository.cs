namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class LoadRepository(TmsDataContext context) : ILoadRepository
{
    /// <inheritdoc />
    public async Task<Load?> GetByIdAsync(Guid loadId, CancellationToken cancellationToken = default)
        => await context.Loads.FirstOrDefaultAsync(e => e.Id == loadId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Load>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Loads.ToListAsync(cancellationToken);

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
}