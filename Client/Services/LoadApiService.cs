namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class LoadApiService(HttpClient httpClient)
{
    public async Task<List<LoadDto>> GetAllAsync()
    {
        return await httpClient.GetFromJsonAsync<List<LoadDto>>(
                   ApiRoutes.Loads.GetAll)
               ?? new List<LoadDto>();
    }

    public async Task<LoadDto?> GetByIdAsync(Guid id)
    {
        return await httpClient.GetFromJsonAsync<LoadDto>(
            ApiRoutes.Loads.GetById(id));
    }

    public async Task<Guid?> CreateAsync(LoadCreateUpdateDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.Loads.Create,
            dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    public async Task<LoadDto?> UpdateAsync(LoadCreateUpdateDto dto)
    {
        var response = await httpClient.PutAsJsonAsync(
            ApiRoutes.Loads.Update,
            dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoadDto>();
    }

    public async Task<LoadDto?> UpdateStatusAsync(LoadStatusUpdateDto dto)
    {
        var response = await httpClient.PatchAsJsonAsync(
            ApiRoutes.Loads.UpdateStatus,
            dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoadDto>();
    }

    public async Task<LoadDto?> AssignDriverAsync(LoadAssignDriverDto dto)
    {
        var response = await httpClient.PatchAsJsonAsync(
            ApiRoutes.Loads.AssignDriver,
            dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoadDto>();
    }

    public async Task<LoadDto?> DessignDriverAsync(Guid id)
    {
        var response = await httpClient.PatchAsJsonAsync(
            ApiRoutes.Loads.DeassignDriver(id),
            id);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoadDto>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync(
            ApiRoutes.Loads.Delete(id));

        return response.IsSuccessStatusCode;
    }
}