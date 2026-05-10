namespace API.Workers;

using Business.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class DriverActivityWorker(
    IServiceScopeFactory scopeFactory,
    IMemoryCache cache,
    ILogger<DriverActivityWorker> logger) : BackgroundService
{
    private const string CacheKeyPrefix = "inactive_notified_";
    private readonly TimeSpan _notificationInterval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Driver Activity Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var activityService = scope.ServiceProvider.GetRequiredService<IDriverActivityService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var inactiveDrivers = await activityService.GetInactiveDriversAsync(stoppingToken);

                foreach (var driverId in inactiveDrivers)
                {
                    if (!cache.TryGetValue($"{CacheKeyPrefix}{driverId}", out _))
                    {
                        await notificationService.PublishDriverInactiveAsync(driverId);

                        cache.Set($"{CacheKeyPrefix}{driverId}", true, _notificationInterval);

                        logger.LogInformation("Sent inactivity alert to driver {DriverId}", driverId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while checking driver activity.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}