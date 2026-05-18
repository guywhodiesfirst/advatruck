namespace Business.Services;

using System.Net.Http.Json;
using System.Web;
using Business.Interfaces;
using Core.Exceptions;
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

            var response = await httpClient.GetFromJsonAsync<NominatimAddressResponse>(url, cancellationToken);

            return response?.Address == null ? "Address not found" : FormatAddress(response.Address);
        }
        catch
        {
            return "Unknown location";
        }
    }

    /// <inheritdoc/>
    public async Task<GeoPoint?> GetLocationAsync(string address, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return null;
        }

        try
        {
            var encodedAddress = HttpUtility.UrlEncode(address);
            var url = $"search?format=jsonv2&q={encodedAddress}&limit=1";

            var response = await httpClient.GetFromJsonAsync<List<NominatimSearchResponse>>(url, cancellationToken);

            var bestMatch = response?.FirstOrDefault();
            if (bestMatch == null)
            {
                return null;
            }

            if (double.TryParse(bestMatch.Lat, System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(bestMatch.Lon, System.Globalization.CultureInfo.InvariantCulture, out var lon))
            {
                return new GeoPoint
                {
                    Latitude = lat,
                    Longitude = lon,
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new TmsException("Failed to retrieve coordinates", ex);
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