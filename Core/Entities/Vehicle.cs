namespace Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Vehicle
{
    public Guid Id { get; set; }

    [StringLength(255)]
    public string Model { get; set; } = null!;

    public int CargoSpaceWidth { get; set; }

    public int CargoSpaceLength { get; set; }

    public int CargoSpaceHeight { get; set; }

    public Guid DriverId { get; set; }

    public Driver Driver { get; set; } = null!;
}
