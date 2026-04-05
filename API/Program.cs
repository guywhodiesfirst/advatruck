using API.Extensions;
using Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up TMS API");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console();
    });
    
    builder.Services.AddControllers();

    builder.Services.AddHttpClient();
    
    builder.Services.AddDbContext<TmsDataContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );

    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TmsDataContext>();
        await dbContext.Database.MigrateAsync();
        
        dbContext.SeedData();
    }
    
    app.UseHttpsRedirection();
    app.UseAuthorization();
    
    app.MapControllers();
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}