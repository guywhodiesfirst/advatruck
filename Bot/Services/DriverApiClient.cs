namespace Bot.Services;

using Bot.Interfaces;
using Core.Entities;
using Core.Models;
using Core.Types;
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
        var request = new TrackingUpdateRequestDto
        {
            Location = new GeoPoint
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            },
            Timestamp = DateTime.UtcNow,
        };
        await http.PostAsJsonAsync($"{_baseUrl}/{driverId}/locations", request);
    }
}