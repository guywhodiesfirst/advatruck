using System.IdentityModel.Tokens.Jwt;
using API.Extensions;
using API.Middlewares;
using Scalar.AspNetCore;
using Serilog;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting up Advatruck TMS API...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());

    builder.Services.AddPresentationApi();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddSecurityAndCors(builder.Configuration);
    builder.Services.AddBusinessServices();
    builder.Services.AddAutoMapper(_ => { }, typeof(Business.MappingProfile));

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseCors("ClientPolicy");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        await app.MigrateAndSeedDatabaseAsync();
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