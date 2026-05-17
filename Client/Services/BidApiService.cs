namespace Client.Services;

using System.Net.Http.Json;
using Client.Routing;
using Core.Models;

public class BidApiService(HttpClient httpClient)
{
    public async Task<List<BidDto>> GetAllAsync() =>
        await httpClient.GetFromJsonAsync<List<BidDto>>(ApiRoutes.Bids.GetAll) ??
            [];

    public async Task<Guid?> PlaceAsync(BidCreateUpdateDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Bids.Place, dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        return null;
    }
}