namespace Bot.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bot.Interfaces;
using Core.Models;
using Core.Types;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

/// <inheritdoc/>
public class ApiClient(
    HttpClient http,
    IOptions<TelegramBotOptions> options) : IApiClient
{
    private readonly string _baseUrl =
        $"{options.Value.BaseApiUrl}/api/v{options.Value.ApiVersion}";

    /// <inheritdoc/>
    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var response = await http.PostAsJsonAsync(
            $"{_baseUrl}/auth/login",
            new { Email = email, Password = password });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    /// <inheritdoc/>
    public async Task<DriverProfileDto?> GetProfileAsync(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/drivers/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadFromJsonAsync<DriverProfileDto>();

        return content;
    }

    /// <inheritdoc/>
    public async Task SendLocationAsync(Guid driverId, Location location, string token)
    {
        var dto = new TrackingUpdateRequestDto
        {
            Location = new GeoPoint
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            },
            Timestamp = DateTime.UtcNow,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/drivers/{driverId}/locations");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(dto);

        await http.SendAsync(request);
    }
}