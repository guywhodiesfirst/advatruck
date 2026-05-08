namespace Business.Services;

using System.Net.Http.Json;
using Business.Interfaces;
using Core.Models;
using Core.Types;

/// <summary>
/// An implementation of <see cref="IGeocodingService"/> that works with Nominatim API.
/// </summary>
public class NominatimService(HttpClient httpClient) : IGeocodingService
{
    /// <inheritdoc/>
    public async Task<string> GetAddressAsync(GeoPoint location, CancellationToken cancellationToken = default)
    {
        try
        {
            var latStr = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var lonStr = location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var url = $"reverse?format=jsonv2&lat={latStr}&lon={lonStr}";

            var response = await httpClient.GetFromJsonAsync<NominatimResponse>(url, cancellationToken);

            if (response?.Address == null)
            {
                return "Address not found";
            }

            return FormatAddress(response.Address);
        }
        catch
        {
            return "Unknown location";
        }
    }

    private static string FormatAddress(NominatimAddress addr)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(addr.State))
        {
            parts.Add(addr.State);
        }

        var city = !string.IsNullOrEmpty(addr.City) ? addr.City :
                   !string.IsNullOrEmpty(addr.Town) ? addr.Town : addr.Village;
        if (!string.IsNullOrEmpty(city))
        {
            parts.Add(city);
        }

        if (!string.IsNullOrEmpty(addr.Road))
        {
            parts.Add(addr.Road);
        }

        if (!string.IsNullOrEmpty(addr.HouseNumber))
        {
            parts.Add(addr.HouseNumber);
        }

        if (!string.IsNullOrEmpty(addr.Postcode))
        {
            parts.Add(addr.Postcode);
        }

        return string.Join(", ", parts);
    }
}