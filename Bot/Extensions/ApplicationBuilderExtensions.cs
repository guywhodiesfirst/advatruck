namespace Bot.Extensions;

using Bot.Options;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

public static class ApplicationBuilderExtensions
{
    public static async Task ConfigureTelegramWebhookAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        try
        {
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<TelegramBotOptions>>().Value;

            await botClient.SetWebhook(options.WebhookUrl);
            Log.Information("Telegram Webhook successfully set to: {Webhook}", options.WebhookUrl);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to set Telegram Webhook during bot startup.");
        }
    }
}