using Core.Types;

namespace Data.Models;

public class DriverLocation
{
    public GeoPoint Location { get; set; }
    public DateTime LastModified { get; set; }
    public Guid DriverId { get; set; }
    public Driver Driver { get; set; } = null!;
}
