namespace Business.Services;

using System.Net;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
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
        var location = await repo.GetLastByDriverIdAsync(driverId, cancellationToken);

        return location
               ?? throw new TmsException($"Last location for driver {driverId} not found", HttpStatusCode.NotFound);
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

        try
        {
            return await repo.AddAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to save driver location", ex, HttpStatusCode.InternalServerError);
        }
    }
}