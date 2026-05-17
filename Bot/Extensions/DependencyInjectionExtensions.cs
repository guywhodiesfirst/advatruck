namespace Bot.Extensions;

using Bot.Handlers;
using Bot.Interfaces;
using Bot.Options;
using Bot.Services;
using Bot.Workers;
using Core.Options;
using Data.Interfaces;
using Data.State;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using Telegram.Bot;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBotOptionsWithValidation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TelegramBotOptions>()
            .Bind(configuration.GetSection(TelegramBotOptions.ConfigurationSection))
            .Validate(o => !string.IsNullOrWhiteSpace(o.TelegramToken), $"{nameof(TelegramBotOptions.TelegramToken)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.WebhookUrl), $"{nameof(TelegramBotOptions.WebhookUrl)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.RedisConnectionString), $"{nameof(TelegramBotOptions.RedisConnectionString)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseApiUrl), $"{nameof(TelegramBotOptions.BaseApiUrl)} is required")
            .Validate(o => o.LocationUpdateIntervalMinutes > 0, $"{nameof(TelegramBotOptions.LocationUpdateIntervalMinutes)} is required");

        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.ConfigurationSection))
            .Validate(o => !string.IsNullOrWhiteSpace(o.HostName), $"{nameof(RabbitMqOptions.HostName)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserName), $"{nameof(RabbitMqOptions.UserName)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), $"{nameof(RabbitMqOptions.Password)} is required")
            .Validate(o => o.Port > 0, $"{nameof(RabbitMqOptions.Port)} must be greater than 0");

        return services;
    }

    public static IServiceCollection AddBotInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = new RabbitMqOptions();
        configuration.Bind(RabbitMqOptions.ConfigurationSection, rabbitOptions);

        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitOptions.HostName,
                UserName = rabbitOptions.UserName,
                Password = rabbitOptions.Password,
                Port = rabbitOptions.Port,
            };
            return factory.CreateConnection();
        });

        var opts = new TelegramBotOptions();
        configuration.Bind(TelegramBotOptions.ConfigurationSection, opts);

        services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(opts.TelegramToken));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));

        return services;
    }

    public static IServiceCollection AddBotApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<UpdateHandler>();

        services.AddSingleton<IDriverSessionStore, DriverSessionStore>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICommandService, CommandService>();
        services.AddSingleton<ITrackingService, TrackingService>();
        services.AddSingleton<ICallbackQueryService, CallbackQueryService>();

        services.AddHostedService<DriverEventConsumer>();

        return services;
    }
}