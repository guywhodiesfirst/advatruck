namespace Client.Services;

using System.Net.Http.Json;
using Client.Auth;
using Client.Routing;
using Core.Models;
using Microsoft.AspNetCore.Components;

public class AuthApiService(
    HttpClient httpClient,
    JwtAuthenticationStateProvider authStateProvider,
    NavigationManager navigation)
{
    public async Task<bool> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            Console.WriteLine($"[AuthApiService] BaseAddress: {httpClient.BaseAddress}");
            Console.WriteLine($"[AuthApiService] Attempting login for: {dto.Email}");
            Console.WriteLine($"[AuthApiService] Endpoint: {ApiRoutes.Auth.Login}");
            Console.WriteLine($"[AuthApiService] Full URL: {httpClient.BaseAddress}{ApiRoutes.Auth.Login}");

            var response = await httpClient.PostAsJsonAsync(
                ApiRoutes.Auth.Login,
                dto);

            Console.WriteLine($"[AuthApiService] Response status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[AuthApiService] Login failed with status: {response.StatusCode}");
                return false;
            }

            var result = await response.Content
                .ReadFromJsonAsync<AuthResponseDto>();

            Console.WriteLine($"[AuthApiService] Response deserialized: {result != null}");

            if (result is null || string.IsNullOrWhiteSpace(result.Token))
            {
                Console.WriteLine($"[AuthApiService] Invalid result or token");
                return false;
            }

            Console.WriteLine($"[AuthApiService] Login successful, marking as authenticated");

            await authStateProvider.MarkUserAsAuthenticatedAsync(result.Token);

            navigation.NavigateTo("/");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthApiService] Exception: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"[AuthApiService] Stack: {ex.StackTrace}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await authStateProvider.MarkUserAsLoggedOutAsync();

        navigation.NavigateTo("/login", forceLoad: true);
    }
}