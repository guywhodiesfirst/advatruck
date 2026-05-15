namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class AdminApiService(HttpClient httpClient)
{
    public async Task<AdminProfileDto?> GetProfileMeAsync() =>
        await httpClient.GetFromJsonAsync<AdminProfileDto>(ApiRoutes.Admins.GetMe);
}