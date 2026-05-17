namespace API.Extensions;

using Data;
using Microsoft.EntityFrameworkCore;

public static class ApplicationBuilderExtensions
{
    public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TmsDataContext>();
            await dbContext.Database.MigrateAsync();
            dbContext.SeedData();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
            logger.LogError(ex, "Error during applying migrations: {Exception}", ex);
        }
    }
}