using Bot;
using Bot.Handlers;
using Bot.Interfaces;
using Bot.Services;
using Bot.State;
using Microsoft.AspNetCore.Mvc;
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

    builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.WriteTo.Console();
    });

    builder.Services
        .AddOptions<TelegramBotOptions>()
        .Bind(builder.Configuration.GetSection(TelegramBotOptions.ConfigurationSection))
        .Validate(o => !string.IsNullOrWhiteSpace(o.TelegramToken),
            $"{nameof(TelegramBotOptions.TelegramToken)} is required")
        .Validate(o => !string.IsNullOrWhiteSpace(o.WebhookUrl),
            $"{nameof(TelegramBotOptions.WebhookUrl)} is required")
        .Validate(o => !string.IsNullOrWhiteSpace(o.RedisConnectionString),
            $"{nameof(TelegramBotOptions.RedisConnectionString)} is required")
        .Validate(o => !string.IsNullOrWhiteSpace(o.BaseApiUrl),
            $"{nameof(TelegramBotOptions.BaseApiUrl)} is required");
    
    var opts = new TelegramBotOptions();
    builder.Configuration.Bind(TelegramBotOptions.ConfigurationSection, opts);

    // Telegram client
    builder.Services.AddSingleton<ITelegramBotClient>(
        _ => new TelegramBotClient(opts.TelegramToken));

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(opts.RedisConnectionString));

    builder.Services.AddSingleton<IDriverSessionStore, DriverSessionStore>();

    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<DriverApiClient>();
    builder.Services.AddSingleton<UpdateHandler>();

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