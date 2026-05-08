namespace Core.Entities;

public class Load
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public required ICollection<LoadLocation> LoadLocations { get; set; } = new List<LoadLocation>();

    public DateTime? ClosedAt { get; set; }

    public decimal Rate { get; set; }

    public decimal DriverCharge { get; set; }

    public Guid DriverId { get; set; }

    public Driver Driver { get; set; } = null!;

    public Guid DispatcherId { get; set; }

    public required Dispatcher Dispatcher { get; set; }

    public double CargoWeight { get; set; }

    public int CargoWidth { get; set; }

    public int CargoLength { get; set; }

    public int CargoHeight { get; set; }
}