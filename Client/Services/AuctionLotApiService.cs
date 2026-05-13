namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class AuctionLotApiService(HttpClient httpClient)
{
    public async Task<List<AuctionLotDto>> GetAllAsync() =>
        await httpClient.GetFromJsonAsync<List<AuctionLotDto>>(ApiRoutes.AuctionLots.GetAll) ??
            [];

    public async Task<List<AuctionLotDto>> GetAllActiveAsync() =>
        await httpClient.GetFromJsonAsync<List<AuctionLotDto>>(ApiRoutes.AuctionLots.GetAllActive) ??
            [];

    public async Task<AuctionLotDto?> GetByIdAsync(Guid id) =>
        await httpClient.GetFromJsonAsync<AuctionLotDto>(ApiRoutes.AuctionLots.GetById(id));

    public async Task<Guid?> CreateAsync(AuctionLotCreateUpdateDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.AuctionLots.Create, dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Guid>()
            : null;
    }

    public async Task<AuctionLotDto?> UpdateAsync(AuctionLotCreateUpdateDto dto)
    {
        var response = await httpClient.PutAsJsonAsync(ApiRoutes.AuctionLots.Update, dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AuctionLotDto>()
            : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync(ApiRoutes.AuctionLots.Delete(id));
        return response.IsSuccessStatusCode;
    }

    public async Task<AuctionLotDto?> UpdateStatusAsync(Guid id, AuctionLotStatusUpdateDto dto)
    {
        var response = await httpClient.PatchAsJsonAsync(ApiRoutes.AuctionLots.UpdateStatus(id), dto);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<AuctionLotDto>()
            : null;
    }
}