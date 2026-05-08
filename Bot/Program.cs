using Bot;
using Bot.Handlers;
using Bot.Interfaces;
using Bot.Services;
using Bot.Workers;
using Data.Interfaces;
using Data.State;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using Serilog;
using StackExchange.Redis;
using Telegram.Bot;
using Telegram.Bot.Types;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up TMS TelegramBot");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((_, lc) =>
    {
        lc.WriteTo.Console();
    });

    builder.Services
        .AddOptions<TelegramBotOptions>()
        .Bind(builder.Configuration.GetSection(TelegramBotOptions.ConfigurationSection))
        .Validate(
            o => !string.IsNullOrWhiteSpace(o.TelegramToken),
            $"{nameof(TelegramBotOptions.TelegramToken)} is required")
        .Validate(
            o => !string.IsNullOrWhiteSpace(o.WebhookUrl),
            $"{nameof(TelegramBotOptions.WebhookUrl)} is required")
        .Validate(
            o => !string.IsNullOrWhiteSpace(o.RedisConnectionString),
            $"{nameof(TelegramBotOptions.RedisConnectionString)} is required")
        .Validate(
            o => !string.IsNullOrWhiteSpace(o.BaseApiUrl),
            $"{nameof(TelegramBotOptions.BaseApiUrl)} is required")
        .Validate(
            o => o.LocationUpdateIntervalMinutes > 0,
            $"{nameof(TelegramBotOptions.LocationUpdateIntervalMinutes)} is required");

    builder.Services.AddSingleton<IConnection>(_ =>
    {
        var factory = new ConnectionFactory
        {
            HostName = builder.Configuration["RabbitMq:HostName"] ?? "localhost",
            UserName = builder.Configuration["RabbitMq:UserName"] ?? "admin",
            Password = builder.Configuration["RabbitMq:Password"] ?? "admin",
            Port = int.TryParse(builder.Configuration["RabbitMq:Port"], out var port) ? port : 5672,
        };

        return factory.CreateConnection();
    });

    var opts = new TelegramBotOptions();
    builder.Configuration.Bind(TelegramBotOptions.ConfigurationSection, opts);

    builder.Services.AddSingleton<ITelegramBotClient>(
        _ => new TelegramBotClient(opts.TelegramToken));

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379"));

    builder.Services.AddSingleton<IDriverSessionStore, DriverSessionStore>();
    builder.Services.AddSingleton<IAuthService, AuthService>();
    builder.Services.AddSingleton<ICommandService, CommandService>();
    builder.Services.AddSingleton<ITrackingService, TrackingService>();

    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IApiClient, ApiClient>();
    builder.Services.AddSingleton<UpdateHandler>();

    builder.Services.AddHostedService<DriverEventConsumer>();

    var app = builder.Build();

    var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
    await botClient.SetWebhook(opts.WebhookUrl);

    Log.Information("Webhook set: {Webhook}", opts.WebhookUrl);

    app.MapPost("/bot-webhook", async (
        [FromBody] Update update,
        UpdateHandler handler) =>
    {
        Log.Information("Update received: {UpdateType}", update.Type);

        await handler.HandleAsync(update);

        return Results.Ok();
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Telegram bot terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}