using Bot.Extensions;
using Bot.Handlers;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Telegram.Bot.Types;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up TMS TelegramBot");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

    builder.Services.AddBotOptionsWithValidation(builder.Configuration);
    builder.Services.AddBotInfrastructure(builder.Configuration);
    builder.Services.AddBotApplicationServices();

    var app = builder.Build();

    await app.ConfigureTelegramWebhookAsync();

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