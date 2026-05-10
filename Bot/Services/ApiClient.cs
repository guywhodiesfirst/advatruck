namespace Bot.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Bot.Interfaces;
using Core.Exceptions;
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
            await HandleErrorResponse(response);
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
            await HandleErrorResponse(response);
        }

        return await response.Content.ReadFromJsonAsync<DriverProfileDto>();
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

        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }
    }

    /// <summary>
    /// Parses error response and throws exception.
    /// </summary>
    /// <param name="response">Error response.</param>
    /// <exception cref="TmsException">TMS Exception.</exception>
    private static async Task HandleErrorResponse(HttpResponseMessage response)
    {
        string errorMessage;
        try
        {
            var errorData = await response.Content.ReadFromJsonAsync<JsonElement>();
            errorMessage = errorData.GetProperty("error").GetString() ?? "Unknown API Error";
        }
        catch
        {
            errorMessage = $"API Error: {response.StatusCode}";
        }

        throw new TmsException(errorMessage, response.StatusCode);
    }

    /// <inheritdoc/>
    public async Task<LoadDto?> GetLoadByIdAsync(Guid loadId, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/loads/{loadId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await http.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        return await response.Content.ReadFromJsonAsync<LoadDto>(options);
    }
}