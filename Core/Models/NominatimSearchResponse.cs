namespace Core.Models;

using System.Text.Json.Serialization;

public class NominatimSearchResponse
{
    [JsonPropertyName("lat")]
    public string Lat { get; set; } = string.Empty;

    [JsonPropertyName("lon")]
    public string Lon { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}