using API;
using API.Extensions;
using API.Notifications;
using API.Workers;
using Asp.Versioning;
using Business.Interfaces;
using Business.Services;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Data.State;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using StackExchange.Redis;

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

    builder.Services.Configure<RabbitMqOptions>(
        builder.Configuration.GetSection("RabbitMq"));

    // TODO: use RabbitMqOptions here
    builder.Services.AddSingleton<IConnection>(_ =>
    {
        var factory = new ConnectionFactory
        {
            HostName = builder.Configuration["RabbitMq:HostName"],
            UserName = builder.Configuration["RabbitMq:UserName"],
            Password = builder.Configuration["RabbitMq:Password"],
            Port = Convert.ToInt32(builder.Configuration["RabbitMq:Port"]),
        };

        return factory.CreateConnection();
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        var config = builder.Configuration.GetConnectionString("Redis")
                     ?? "localhost:6379";

        return ConnectionMultiplexer.Connect(config);
    });

    builder.Services.AddSingleton<IModel>(sp =>
    {
        var connection = sp.GetRequiredService<IConnection>();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "driver.inactive",
            durable: true,
            exclusive: false,
            autoDelete: false);

        return channel;
    });

    builder.Services.AddDbContext<TmsDataContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IDriverRepository, DriverRepository>();
    builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
    builder.Services.AddScoped<IDriverSessionStore, DriverSessionStore>();

    builder.Services.AddScoped<IDriverService, DriverService>();
    builder.Services.AddScoped<IDriverActivityService, DriverActivityService>();
    builder.Services.AddScoped<IDriverLocationService, DriverLocationService>();

    builder.Services.AddScoped<DriverEventPublisher>();

    builder.Services.AddHostedService<DriverActivityWorker>();

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