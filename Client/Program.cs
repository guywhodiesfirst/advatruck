using Blazored.LocalStorage;
using Client;
using Client.Auth;
using Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddMudServices();

builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<LoadApiService>();
builder.Services.AddScoped<AuctionLotApiService>();
builder.Services.AddScoped<BidApiService>();
builder.Services.AddScoped<DriverApiService>();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddScoped<JwtAuthorizationHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<JwtAuthorizationHandler>();
    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5112/"),
    };
});

await builder.Build().RunAsync();