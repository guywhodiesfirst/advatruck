using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using API;
using API.Extensions;
using API.Middlewares;
using API.Workers;
using Asp.Versioning;
using Business;
using Business.Interfaces;
using Business.Services;
using Core.Identity;
using Data;
using Data.Interfaces;
using Data.Repositories;
using Data.State;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up Advatruck TMS API...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((_, lc) =>
    {
        lc.WriteTo.Console();
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    builder.Services.AddHttpClient();

    builder.Services.Configure<RabbitMqOptions>(
        builder.Configuration.GetSection("RabbitMq"));

    // TODO: use RabbitMqOptions here
    builder.Services.AddSingleton<IConnection>(_ =>
    {
        var factory = new ConnectionFactory
        {
            HostName = builder.Configuration["RabbitMq:HostName"],
            UserName = builder.Configuration["RabbitMq:UserName"],
            Password = builder.Configuration["RabbitMq:Password"],
            Port = Convert.ToInt32(builder.Configuration["RabbitMq:Port"]),
        };

        return factory.CreateConnection();
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        var config = builder.Configuration.GetConnectionString("Redis")
                     ?? "localhost:6379";

        return ConnectionMultiplexer.Connect(config);
    });

    builder.Services.AddSingleton<IModel>(sp =>
    {
        var connection = sp.GetRequiredService<IConnection>();
        var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "driver.inactive",
            durable: true,
            exclusive: false,
            autoDelete: false);

        return channel;
    });

    builder.Services.AddDbContext<TmsDataContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<TmsDataContext>();

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = false,
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claims = context.Principal?.Claims ??
                                 [];
                    Console.WriteLine("Token validated! Claims found:");
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"{claim.Type}: {claim.Value}");
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = _ =>
                {
                    Console.WriteLine("Forbidden error! User is authenticated but doesn't have the right role.");
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Auth failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
            };
        });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ClientPolicy", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IDriverRepository, DriverRepository>();
    builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
    builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
    builder.Services.AddScoped<ILoadRepository, LoadRepository>();
    builder.Services.AddScoped<IDispatcherRepository, DispatcherRepository>();
    builder.Services.AddScoped<IAuctionLotRepository, AuctionLotRepository>();
    builder.Services.AddScoped<IBidRepository, BidRepository>();
    builder.Services.AddScoped<IAdminRepository, AdminRepository>();

    builder.Services.AddScoped<IDriverService, DriverService>();
    builder.Services.AddScoped<IGeocodingService, NominatimService>();
    builder.Services.AddScoped<IDriverActivityService, DriverActivityService>();
    builder.Services.AddScoped<IDriverLocationService, DriverLocationService>();
    builder.Services.AddScoped<IVehicleService, VehicleService>();
    builder.Services.AddScoped<ILoadService, LoadService>();
    builder.Services.AddScoped<IDispatcherService, DispatcherService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IAuctionLotService, AuctionLotService>();
    builder.Services.AddScoped<IBidService, BidService>();
    builder.Services.AddScoped<IAdminService, AdminService>();

    builder.Services.AddSingleton<IDriverSessionStore, DriverSessionStore>();

    builder.Services.AddMemoryCache();
    builder.Services.AddHostedService<DriverActivityWorker>();
    builder.Services.AddHostedService<LoadStatusWorker>();
    builder.Services.AddHostedService<AuctionStatusWorker>();

    builder.Services.AddHttpClient<IGeocodingService, NominatimService>(c =>
    {
        c.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        c.DefaultRequestHeaders.Add("User-Agent", "TmsLogisticsBot/1.0");
    });

    builder.Services.AddApiVersioning(options =>
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

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseCors("ClientPolicy");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TmsDataContext>();
            await dbContext.Database.MigrateAsync();

            dbContext.SeedData();
        }
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Advatruck TMS API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}