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
        var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.Auth.Login,
            dto);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content
            .ReadFromJsonAsync<AuthResponseDto>();

        if (result is null || string.IsNullOrWhiteSpace(result.Token))
        {
            return false;
        }

        await authStateProvider.MarkUserAsAuthenticatedAsync(result.Token);

        navigation.NavigateTo("/");

        return true;
    }

    public async Task LogoutAsync()
    {
        await authStateProvider.MarkUserAsLoggedOutAsync();

        navigation.NavigateTo("/login", forceLoad: true);
    }
}