namespace Data.Repositories;

using Core.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class VehicleRepository(TmsDataContext context) : IVehicleRepository
{
    /// <inheritdoc />
    public async Task<Vehicle?> GetByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        return await context.Vehicles
            .Include(v => v.Driver)
            .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(e => e.Id == vehicleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Vehicles
            .Include(v => v.Driver)
            .ThenInclude(d => d.User)
            .ToListAsync(cancellationToken);
    }

/// <inheritdoc />
    public async Task<Guid> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await context.Vehicles.AddAsync(vehicle, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return vehicle.Id;
    }

    /// <inheritdoc />
    public async Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        context.Vehicles.Remove(vehicle);
        await context.SaveChangesAsync(cancellationToken);
    }
}