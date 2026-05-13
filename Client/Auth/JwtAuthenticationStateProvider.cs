namespace Client.Auth;

using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class JwtAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(_anonymous);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    public async Task<string?> GetTokenAsync() => await localStorage.GetItemAsync<string>("authToken");

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await localStorage.SetItemAsync("authToken", token);
        var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"))));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await localStorage.RemoveItemAsync("authToken");
        var authState = Task.FromResult(new AuthenticationState(_anonymous));
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var claims = new List<Claim>();

        if (keyValuePairs!.TryGetValue("role", out var roles))
        {
            var rolesString = roles.ToString();
            if (rolesString!.Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                foreach (var role in parsedRoles!)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, rolesString));
            }

            keyValuePairs.Remove("role");
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}