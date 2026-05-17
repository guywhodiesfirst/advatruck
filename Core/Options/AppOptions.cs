namespace Core.Options;

public class AppOptions
{
    public const string ConfigName = "App";

    public required string PostgresConnectionString { get; set; }

    public required string RedisConnectionString { get; set; }
}