namespace Core.Enums;

using System.Text.Json.Serialization;

/// <summary>
/// Represents load location type.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoadLocationType
{
    /// <summary>
    /// Load pickup location.
    /// </summary>
    Pickup,

    /// <summary>
    /// Load delivery location.
    /// </summary>
    Delivery,
}