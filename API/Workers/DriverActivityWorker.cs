namespace API.Workers;

using API.Notifications;
using Business.Interfaces;

public class DriverActivityWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();

            var activity = scope.ServiceProvider.GetRequiredService<IDriverActivityService>();
            var publisher = scope.ServiceProvider.GetRequiredService<DriverEventPublisher>();

            var inactive = await activity.GetInactiveDriversAsync(cancellationToken);

            foreach (var driverId in inactive)
            {
                publisher.PublishDriverInactive(driverId);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
}