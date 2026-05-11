namespace Core.Models;

using Core.Types;

public record TrackingUpdateRequestDto
{
    public required GeoPoint Location { get; set; }

    public required DateTime Timestamp { get; set; }
}