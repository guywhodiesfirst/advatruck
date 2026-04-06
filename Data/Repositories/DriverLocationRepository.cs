using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc/>
public class DriverLocationRepository(TmsDataContext context)
    : IDriverLocationRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<DriverLocation>> GetAllByDriverIdAsync(Guid driverId, 
        CancellationToken cancellationToken = default)
    {
        return await context.DriverLocations
            .Where(dl => dl.DriverId == driverId)
            .Include(dl => dl.Driver)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DriverLocation?> GetLastByDriverIdAsync(Guid driverId, 
        CancellationToken cancellationToken = default)
    {
        return await context.DriverLocations
            .Where(dl => dl.DriverId == driverId)
            .OrderByDescending(dl => dl.UpdateTime)
            .Include(dl => dl.Driver)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DriverLocation> AddAsync(DriverLocation driverLocation,
        CancellationToken cancellationToken = default)
    {
        driverLocation.UpdateTime = DateTime.Now;
        await context.DriverLocations.AddAsync(driverLocation,  cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return driverLocation;
    }
}