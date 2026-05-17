#nullable disable

namespace Bot.Options;

public class TelegramBotOptions
{
    public const string ConfigurationSection = "TelegramBot";

    public string TelegramToken { get; set; }

    public string WebhookUrl { get; set; }

    public string RedisConnectionString { get; set; }

    public string BaseApiUrl { get; set; }

    public string ApiVersion { get; set; } = "1";

    public int LocationUpdateIntervalMinutes { get; set; } = 5;
}