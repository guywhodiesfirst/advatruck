namespace Core.Models;

using Core.Entities;
using Core.Types;

public class DriverLocationDto
{
    public required GeoPoint Location { get; set; }

    public DateTime UpdateTime { get; set; }

    public Guid DriverId { get; set; }

    public string Address { get; set; } = string.Empty;
}