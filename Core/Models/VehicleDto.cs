namespace Core.Models;

public class VehicleDto
{
    public Guid Id { get; set; }

    public string Model { get; set; } = string.Empty;

    public string PlateNumber { get; set; } = string.Empty;

    public int CargoSpaceWidth { get; set; }

    public int CargoSpaceLength { get; set; }

    public int CargoSpaceHeight { get; set; }

    public int MaxWeight { get; set; }

    public Guid DriverId { get; set; }

    public string DriverName { get; set; } = string.Empty;
}