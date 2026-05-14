namespace API.Workers;

using Core.Enums;
using Data;
using Microsoft.EntityFrameworkCore;

public class AuctionStatusWorker(
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<TmsDataContext>();

            var expiredAuctions = await dbContext.AuctionLots
                .Where(x =>
                    x.Status == AuctionStatus.Active &&
                    x.EndsAt <= DateTime.UtcNow)
                .ToListAsync(stoppingToken);

            foreach (var auction in expiredAuctions)
            {
                auction.Status = AuctionStatus.Finished;
            }

            if (expiredAuctions.Count > 0)
            {
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}