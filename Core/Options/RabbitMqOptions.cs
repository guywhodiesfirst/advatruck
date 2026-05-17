#nullable disable

namespace Core.Options;

public class RabbitMqOptions
{
    public const string ConfigurationSection = "RabbitMq";

    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public int Port { get; set; }
}