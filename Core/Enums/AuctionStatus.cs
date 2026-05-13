namespace Core.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuctionStatus
{
    /// <summary>
    /// Active auction.
    /// </summary>
    Active,

    /// <summary>
    /// Finished auction.
    /// </summary>
    Finished,
}