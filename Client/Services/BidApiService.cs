namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class BidApiService(HttpClient httpClient)
{
    public async Task<List<BidDto>> GetAllAsync() =>
        await httpClient.GetFromJsonAsync<List<BidDto>>(ApiRoutes.Bids.GetAll) ??
            [];

    public async Task<Guid?> CreateAsync(BidCreateUpdateDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Bids.Create, dto);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Guid>() : null;
    }
}