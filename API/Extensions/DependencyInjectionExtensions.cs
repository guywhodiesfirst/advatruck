namespace API.Extensions;

using System.Text;
using System.Text.Json.Serialization;
using API.Options;
using API.Workers;
using Asp.Versioning;
using Business.Interfaces;
using Business.Services;
using Core.Identity;
using Core.Options;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Data.State;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddPresentationApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddHttpClient();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("apiVersion"));
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddOpenApi();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.ConfigurationSection))
            .Validate(o => !string.IsNullOrWhiteSpace(o.HostName), $"{nameof(RabbitMqOptions.HostName)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserName), $"{nameof(RabbitMqOptions.UserName)} is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), $"{nameof(RabbitMqOptions.Password)} is required")
            .Validate(o => o.Port > 0, $"{nameof(RabbitMqOptions.Port)} must be greater than 0")
            .ValidateOnStart();

        services.AddOptions<ApiOptions>()
            .Bind(configuration.GetSection(ApiOptions.ConfigName))
            .ValidateOnStart();

        var rabbitOptions = configuration.GetSection(RabbitMqOptions.ConfigurationSection).Get<RabbitMqOptions>();
        var appOptions = configuration.GetSection(ApiOptions.ConfigName).Get<ApiOptions>();

        services.AddSingleton<IConnection>(_ => new ConnectionFactory
        {
            HostName = rabbitOptions!.HostName,
            UserName = rabbitOptions.UserName,
            Password = rabbitOptions.Password,
            Port = rabbitOptions.Port,
        }.CreateConnection());

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(appOptions!.RedisConnectionString));

        services.AddSingleton<IModel>(sp =>
        {
            var connection = sp.GetRequiredService<IConnection>();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "driver.inactive", durable: true, exclusive: false, autoDelete: false);
            return channel;
        });

        services.AddDbContext<TmsDataContext>(options =>
            options.UseNpgsql(appOptions!.PostgresConnectionString));

        return services;
    }

    public static IServiceCollection AddSecurityAndCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<TmsDataContext>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = false,
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = _ =>
                    {
                        Console.WriteLine("Token validated!");
                        return Task.CompletedTask;
                    },
                    OnForbidden = _ =>
                    {
                        Console.WriteLine("Forbidden error!");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Auth failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                };
            });

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
                             ["http://localhost:3000"];

        services.AddCors(options =>
        {
            options.AddPolicy("ClientPolicy", policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<ILoadRepository, LoadRepository>();
        services.AddScoped<IDispatcherRepository, DispatcherRepository>();
        services.AddScoped<IAuctionLotRepository, AuctionLotRepository>();
        services.AddScoped<IBidRepository, BidRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IDriverActivityService, DriverActivityService>();
        services.AddScoped<IDriverLocationService, DriverLocationService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<ILoadService, LoadService>();
        services.AddScoped<IDispatcherService, DispatcherService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuctionLotService, AuctionLotService>();
        services.AddScoped<IBidService, BidService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddSingleton<IDriverSessionStore, DriverSessionStore>();
        services.AddMemoryCache();

        services.AddHostedService<DriverActivityWorker>();
        services.AddHostedService<LoadStatusWorker>();
        services.AddHostedService<AuctionStatusWorker>();

        services.AddHttpClient<IGeocodingService, NominatimService>(c =>
        {
            c.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
            c.DefaultRequestHeaders.Add("User-Agent", "TmsLogisticsBot/1.0");
        });

        return services;
    }
}