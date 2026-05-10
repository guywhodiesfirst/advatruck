namespace API.Workers;

using Business.Interfaces;
using Core.Enums;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class LoadStatusWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<LoadStatusWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Load Status Background Worker started.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var loadRepository = scope.ServiceProvider.GetRequiredService<ILoadRepository>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var scheduledLoads = await loadRepository.GetAllByStatusAsync(LoadStatus.Scheduled, cancellationToken);
                var utcNow = DateTime.UtcNow;

                foreach (var load in scheduledLoads)
                {
                    var firstStop = load.LoadStops.OrderBy(s => s.Timestamp).FirstOrDefault();

                    if (firstStop == null || utcNow < firstStop.Timestamp)
                    {
                        continue;
                    }

                    logger.LogInformation("Automatic load status update for {LoadId}: Scheduled -> Ongoing", load.Id);

                    load.LoadStatus = LoadStatus.Ongoing;
                    await loadRepository.UpdateAsync(load, cancellationToken);

                    if (!load.DriverId.HasValue)
                    {
                        continue;
                    }

                    try
                    {
                        await notificationService.PublishLoadAssignedAsync(load.DriverId.Value, load.Id);
                        logger.LogInformation("Trip start notification sent to driver {DriverId}", load.DriverId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to send notification for load {LoadId}", load.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during automatic load status update.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
}