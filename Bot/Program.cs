using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Concurrent;
using Bot;
using Serilog;

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

    var opts = new TelegramBotOptions();
    builder.Configuration.Bind(TelegramBotOptions.ConfigurationSection, opts);

    builder.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(opts.Token));
    builder.Services.AddHttpClient();

    var app = builder.Build();

    var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
    await botClient.SetWebhook(opts.WebhookUrl);

    Console.WriteLine($"Webhook set: {opts.WebhookUrl}");

    var registeredDrivers = new ConcurrentDictionary<long, bool>();

    app.MapPost("/bot-webhook", async (
        [FromBody] Update update,
        ITelegramBotClient client,
        IHttpClientFactory httpFactory) =>
    {
        Log.Information("Update received: {UpdateType}", update.Type);
        if (update.Type != UpdateType.Message || update.Message == null)
            return Results.Ok();

        var message = update.Message;
        var chatId = message.Chat.Id;

        if (message.Location != null)
        {
            if (!registeredDrivers.ContainsKey(chatId))
            {
                await client.SendMessage(chatId, "Спочатку авторизуйся через /start");
                return Results.Ok();
            }

            var lat = message.Location.Latitude;
            var lon = message.Location.Longitude;

            var httpClient = httpFactory.CreateClient();

            await client.SendMessage(
                chatId,
                $"✅ Локацію оновлено:\nLat: {lat}\nLon: {lon}"
            );

            return Results.Ok();
        }

        if (message.Text is not { } text)
            return Results.Ok();

        var keyboard = new ReplyKeyboardMarkup(
        [
            [
                KeyboardButton.WithRequestLocation("📍 Надіслати локацію")
            ],
            [
                new KeyboardButton("🚚 Почати рейс"),
                new KeyboardButton("⛔ Завершити рейс")
            ]
        ])
        {
            ResizeKeyboard = true
        };

        switch (text)
        {
            case "/start":
                registeredDrivers[chatId] = true;

                await client.SendMessage(
                    chatId,
                    "👋 Вітаю, водію!\nНатисни кнопку, щоб відправити локацію",
                    replyMarkup: keyboard
                );
                break;

            case "🚚 Почати рейс":
                registeredDrivers[chatId] = true;

                await client.SendMessage(
                    chatId,
                    "Рейс розпочато 🚀\nУвімкни live location або надсилай локацію вручну",
                    replyMarkup: keyboard
                );
                break;

            case "⛔ Завершити рейс":
                registeredDrivers.TryRemove(chatId, out _);

                await client.SendMessage(
                    chatId,
                    "Рейс завершено ✅",
                    replyMarkup: keyboard
                );
                break;

            default:
                await client.SendMessage(
                    chatId,
                    "Використай кнопки нижче 👇",
                    replyMarkup: keyboard
                );
                break;
        }

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