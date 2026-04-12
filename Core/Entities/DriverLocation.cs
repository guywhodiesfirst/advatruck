namespace Core.Entities;

using Core.Types;

public class DriverLocation
{
    public required GeoPoint Location { get; set; }

    public DateTime UpdateTime { get; set; }

    public Guid DriverId { get; set; }

    public Driver Driver { get; set; } = null!;
}