namespace Core.Entities;

using Core.Enums;
using Core.Types;

public class LoadLocation
{
    public Guid Id { get; set; }

    public LoadLocationType LoadLocationType { get; set; }

    public DateTime Timestamp { get; set; }

    public string Address { get; set; } = string.Empty;

    public required GeoPoint Location { get; set; }
}