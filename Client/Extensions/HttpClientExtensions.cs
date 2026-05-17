namespace Client.Extensions;

using System.Net.Http.Json;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> SendJsonPatchAsync<T>(this HttpClient client, string requestUri, T value)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
        {
            Content = JsonContent.Create(value),
        };
        return await client.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> SendEmptyPatchAsync(this HttpClient client, string requestUri)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);
        return await client.SendAsync(request);
    }
}