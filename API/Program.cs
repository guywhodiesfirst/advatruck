using API.Extensions;
using Asp.Versioning;
using Business.Interfaces;
using Business.Services;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up Advatruck TMS API...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((_, lc) =>
    {
        lc.WriteTo.Console();
    });

    builder.Services.AddControllers();

    builder.Services.AddHttpClient();

    builder.Services.AddDbContext<TmsDataContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IDriverRepository, DriverRepository>();
    builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();

    builder.Services.AddScoped<IDriverService, DriverService>();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("apiVersion"));
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

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
    Log.Fatal(ex, "Advatruck TMS API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}