namespace Business.Services;

using Business.Interfaces;
using Core.Entities;
using Core.Models;
using Data.Interfaces;

public class DriverLocationService(IDriverLocationRepository repo)
    : IDriverLocationService
{
    /// <inheritdoc/>
    public async Task<DriverLocation?> GetLastAsync(
        Guid driverId,
        CancellationToken cancellationToken = default)
    {
        return await repo.GetLastByDriverIdAsync(driverId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DriverLocation> AddAsync(
        Guid driverId,
        TrackingUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = new DriverLocation
        {
            DriverId = driverId,
            Location = request.Location,
            UpdateTime = request.Timestamp,
        };

        return await repo.AddAsync(entity, cancellationToken);
    }
}