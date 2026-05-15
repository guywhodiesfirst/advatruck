namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class UserApiService(HttpClient httpClient)
{
    public async Task<List<UserProfileDto>> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<UserProfileDto>>(ApiRoutes.Users.GetAll) ?? new();
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegistrationRequestDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Users.Register, dto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }
}