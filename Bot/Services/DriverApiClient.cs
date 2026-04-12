namespace Bot.Services;

using Bot.Interfaces;
using Core.Entities;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

/// <inheritdoc/>
public class DriverApiClient(
    HttpClient http,
    IOptions<TelegramBotOptions> options) : IDriverApiClient
{
    private readonly string _baseUrl =
        $"{options.Value.BaseApiUrl}/api/v{options.Value.ApiVersion}/drivers";

    /// <inheritdoc/>
    public async Task<Driver?> LoginAsync(string email)
    {
        var response = await http.PostAsJsonAsync(
            $"{_baseUrl}/login",
            new { Email = email });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Driver>();
    }

    /// <inheritdoc/>
    public async Task SendLocationAsync(Guid driverId, Location location)
    {
        await http.PostAsJsonAsync(
            $"{_baseUrl}/location",
            new
            {
                DriverId = driverId,
                location.Latitude,
                location.Longitude,
                Timestamp = DateTime.UtcNow,
            });
    }
}