namespace Business.Services;

using System.Net;
using Business.Interfaces;
using Core.Exceptions;
using Data.Interfaces;

/// <inheritdoc/>
public class DriverActivityService(
    IDriverSessionStore sessions,
    IDriverRepository repo)
    : IDriverActivityService
{
    /// <inheritdoc/>
    public async Task<List<Guid>> GetInactiveDriversAsync(CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var activeDrivers = await repo.GetAllInTripAsync(cancellationToken);

            var result = new List<Guid>();

            foreach (var d in activeDrivers)
            {
                var last = await sessions.GetLastLocationUpdateAsync(d.Id);

                if (last == null)
                {
                    continue;
                }

                // TODO: replace hardcoded value with options
                if (now - last > TimeSpan.FromMinutes(15))
                {
                    result.Add(d.Id);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to retrieve inactive drivers list", ex, HttpStatusCode.InternalServerError);
        }
    }
}