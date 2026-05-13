namespace Client.Auth;

using System.Net.Http.Headers;
using Blazored.LocalStorage;

public class InterceptorTokenHandler(ISyncLocalStorageService localStorage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = localStorage.GetItem<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}