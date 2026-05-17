namespace Bot.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bot.Interfaces;
using Bot.Options;
using Core.Exceptions;
using Core.Models;
using Core.Types;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

public class ApiClient(
    HttpClient http,
    IOptions<TelegramBotOptions> options) : IApiClient
{
    private readonly string _baseUrl = $"{options.Value.BaseApiUrl}/api/v{options.Value.ApiVersion}";

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <inheritdoc/>
    public async Task<AuthResponseDto?> LoginAsync(string email, string password)
    {
        var response = await http.PostAsJsonAsync($"{_baseUrl}/auth/login", new { Email = email, Password = password });

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);
    }

    /// <inheritdoc/>
    public async Task<DriverProfileDto?> GetProfileAsync(string token)
    {
        return await SendAuthenticatedRequestAsync<DriverProfileDto>(HttpMethod.Get, $"{_baseUrl}/drivers/me", token);
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

        await SendAuthenticatedRequestAsync(HttpMethod.Post, $"{_baseUrl}/drivers/{driverId}/locations", token, dto);
    }

    /// <inheritdoc/>
    public async Task<LoadDto?> GetLoadByIdAsync(Guid loadId, string token)
    {
        try
        {
            return await SendAuthenticatedRequestAsync<LoadDto>(HttpMethod.Get, $"{_baseUrl}/loads/{loadId}", token);
        }
        catch (TmsException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    private async Task<T?> SendAuthenticatedRequestAsync<T>(HttpMethod method, string url, string token, object? body = null)
    {
        using var request = CreateRequest(method, url, token, body);
        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    private async Task SendAuthenticatedRequestAsync(HttpMethod method, string url, string token, object? body = null)
    {
        using var request = CreateRequest(method, url, token, body);
        var response = await http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponse(response);
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, string token, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body != null)
        {
            request.Content = JsonContent.Create(body, options: _jsonOptions);
        }

        return request;
    }

    private static async Task HandleErrorResponse(HttpResponseMessage response)
    {
        string errorMessage;
        try
        {
            var errorData = await response.Content.ReadFromJsonAsync<JsonElement>();
            errorMessage = errorData.TryGetProperty("error", out var errorProp)
                ? errorProp.GetString() ?? "Unknown API Error"
                : "Unknown API Error";
        }
        catch
        {
            errorMessage = $"API Error: {response.StatusCode}";
        }

        throw new TmsException(errorMessage, response.StatusCode);
    }
}