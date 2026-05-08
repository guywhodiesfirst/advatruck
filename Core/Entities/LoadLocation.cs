namespace Core.Entities;

using Core.Enums;
using Core.Types;

public class LoadLocation
{
    public Guid Id { get; set; }

    public LoadLocationType LoadLocationType { get; set; }

    public DateTime Timestamp { get; set; }

    public required GeoPoint Location { get; set; }
}