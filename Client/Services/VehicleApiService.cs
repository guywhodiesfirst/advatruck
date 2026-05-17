namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class VehicleApiService(HttpClient httpClient)
{
    public async Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<IEnumerable<VehicleDto>>(
            ApiRoutes.Vehicles.GetAll,
            cancellationToken);

        return response ?? Enumerable.Empty<VehicleDto>();
    }

    public async Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<VehicleDto>(
            ApiRoutes.Vehicles.GetById(id),
            cancellationToken);
    }

    public async Task<Guid> CreateAsync(VehicleCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Vehicles.Create, dto, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
    }

    public async Task<VehicleDto> UpdateAsync(Guid id, VehicleCreateUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync(ApiRoutes.Vehicles.Update(id), dto, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<VehicleDto>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to deserialize updated vehicle.");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync(ApiRoutes.Vehicles.Delete(id), cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}