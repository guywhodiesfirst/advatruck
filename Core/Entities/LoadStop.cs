namespace Core.Entities;

using Core.Enums;
using Core.Types;

public class LoadStop
{
    public LoadLocationType LoadLocationType { get; set; }

    public DateTime Timestamp { get; set; }

    public required string Address { get; set; }

    public required Guid LoadId { get; set; }

    public string? Note { get; set; }

    public required GeoPoint Location { get; set; }
}