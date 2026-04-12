namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc/>
public class DriverRepository(TmsDataContext context) : IDriverRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Driver>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Drivers
            .Include(d => d.DriverLocations)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Drivers
            .Include(d => d.DriverLocations)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Driver?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Drivers
            .Include(d => d.DriverLocations)
            .FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Guid> AddAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        driver.RegistrationDate = DateTime.Now;
        await context.Drivers.AddAsync(driver, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return driver.Id;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        context.Drivers.Remove(driver);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        context.Drivers.Update(driver);
        await context.SaveChangesAsync(cancellationToken);
        return driver;
    }
}