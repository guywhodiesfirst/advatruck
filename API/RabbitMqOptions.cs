namespace API;

public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";

    public string UserName { get; set; } = "admin";

    public string Password { get; set; } = "admin";

    public int Port { get; set; } = 5672;
}