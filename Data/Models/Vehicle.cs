namespace Data.Models;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Model { get; set; } = null!;
    public int CargoSpaceWidth { get; set; }
    public int CargoSpaceLength { get; set; }
    public int CargoSpaceHeight { get; set; }
    public Guid DrivedId { get; set; }
    public Driver Driver { get; set; } = null!;
}
