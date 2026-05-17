namespace API.Options;

public class ApiOptions
{
    public const string ConfigName = "App";

    public required string PostgresConnectionString { get; set; }

    public required string RedisConnectionString { get; set; }
}