#nullable disable

namespace Bot;

public class TelegramBotOptions
{
    public const string ConfigurationSection = "TelegramBot";
    public string Token { get; set; }
    public string WebhookUrl { get; set; }
}