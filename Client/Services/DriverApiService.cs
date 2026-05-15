namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class DriverApiService(HttpClient httpClient)
{
    public async Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<DriverDto>>(ApiRoutes.Drivers.GetAll, ct)
               ?? Enumerable.Empty<DriverDto>();
    }

    public async Task<DriverDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<DriverDto>(ApiRoutes.Drivers.GetById(id), ct);
    }

    public async Task<DriverProfileDto?> GetProfileMeAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<DriverProfileDto>(ApiRoutes.Drivers.GetMe, ct);
    }

    public async Task<DriverProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<DriverProfileDto>(ApiRoutes.Drivers.GetProfileById(id), ct);
    }

    public async Task<DriverDto?> UpdateAsync(DriverCreateUpdateDto dto, CancellationToken ct = default)
    {
        var response = await httpClient.PutAsJsonAsync(ApiRoutes.Drivers.Update, dto, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DriverDto>(cancellationToken: ct);
    }

    public async Task<LoadDto?> GetActiveLoadAsync(Guid driverId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync(ApiRoutes.Drivers.GetActiveLoad(driverId), ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoadDto>(cancellationToken: ct);
    }
}