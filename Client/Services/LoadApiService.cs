namespace Client.Services;

using System.Net.Http.Json;
using Client.Extensions;
using Client.Routing;
using Core.Models;

public class LoadApiService(HttpClient httpClient)
{
    public async Task<List<LoadDto>> GetAllAsync() =>
        await httpClient.GetFromJsonAsync<List<LoadDto>>(ApiRoutes.Loads.GetAll) ??
            [];

    public async Task<LoadDto?> GetByIdAsync(Guid id) =>
        await httpClient.GetFromJsonAsync<LoadDto>(ApiRoutes.Loads.GetById(id));

    public async Task<Guid?> CreateAsync(LoadCreateUpdateDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Loads.Create, dto);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Guid>() : null;
    }

    public async Task<LoadDto?> UpdateAsync(Guid id, LoadCreateUpdateDto dto)
    {
        var response = await httpClient.PutAsJsonAsync(ApiRoutes.Loads.Update(id), dto);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LoadDto>() : null;
    }

    public async Task<LoadDto?> UpdateStatusAsync(Guid id, LoadStatusUpdateDto dto)
    {
        var response = await httpClient.SendJsonPatchAsync(ApiRoutes.Loads.UpdateStatus(id), dto);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LoadDto>() : null;
    }

    public async Task<LoadDto?> AssignDriverAsync(Guid id, LoadAssignDriverDto dto)
    {
        var response = await httpClient.SendJsonPatchAsync(ApiRoutes.Loads.AssignDriver(id), dto);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LoadDto>() : null;
    }

    public async Task<LoadDto?> DessignDriverAsync(Guid id)
    {
        var response = await httpClient.SendEmptyPatchAsync(ApiRoutes.Loads.DeassignDriver(id));
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<LoadDto>() : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync(ApiRoutes.Loads.Delete(id));
        return response.IsSuccessStatusCode;
    }
}