using Client.Routing;

namespace Client.Services;

using System.Net.Http.Json;
using Core.Models;

public class DispatcherApiService(HttpClient httpClient)
{
    public async Task<DispatcherProfileDto?> GetProfileMeAsync() =>
        await httpClient.GetFromJsonAsync<DispatcherProfileDto>(ApiRoutes.Dispatchers.GetMe);
}