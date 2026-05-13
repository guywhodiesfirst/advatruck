namespace Core.Enums;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the status of the load.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoadStatus
{
    /// <summary>
    /// Scheduled load.
    /// </summary>
    Scheduled,

    /// <summary>
    /// Ongoing load.
    /// </summary>
    Ongoing,

    /// <summary>
    /// Completed load.
    /// </summary>
    Completed,

    /// <summary>
    /// Cancelled load.
    /// </summary>
    Cancelled,
}