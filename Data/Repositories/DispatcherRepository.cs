namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class DispatcherRepository(TmsDataContext context) : IDispatcherRepository
{
    /// <inheritdoc/>
    public async Task<Dispatcher?> GetByIdAsync(Guid dispatcherId, CancellationToken cancellationToken = default)
        => await context.Dispatchers
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == dispatcherId, cancellationToken);

    /// <inheritdoc/>
    public async Task<Dispatcher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Dispatchers
            .Include(d => d.User)
            .Include(d => d.Loads)
            .FirstOrDefaultAsync(d => d.User.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Dispatcher>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Dispatchers
            .Include(e => e.User)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default)
    {
        await context.Dispatchers.AddAsync(dispatcher, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return dispatcher.Id;
    }

    /// <inheritdoc />
    public async Task<Dispatcher> UpdateAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default)
    {
        context.Dispatchers.Update(dispatcher);
        await context.SaveChangesAsync(cancellationToken);
        return dispatcher;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Dispatcher dispatcher, CancellationToken cancellationToken = default)
    {
        context.Dispatchers.Remove(dispatcher);
        await context.SaveChangesAsync(cancellationToken);
    }
}