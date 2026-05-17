namespace Core.Options;

public class RabbitMqOptions
{
    public const string ConfigName = "RabbitMq";

    public required string HostName { get; set; }

    public required string UserName { get; set; }

    public required string Password { get; set; }

    public int Port { get; set; }
}