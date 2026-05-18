namespace Core.Models;

using System.Text.Json.Serialization;

public class NominatimAddressResponse
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public NominatimAddress? Address { get; set; }
}