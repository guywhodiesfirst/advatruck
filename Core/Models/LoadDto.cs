namespace Core.Models;

using Core.Enums;

public class LoadDto
{
    public Guid Id { get; set; }

    public LoadStatus LoadStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public decimal Rate { get; set; }

    public decimal? DriverCharge { get; set; }

    public Guid? DriverId { get; set; }

    public string DriverName { get; set; } = string.Empty;

    public string Note { get; set; } = null!;

    public Guid DispatcherId { get; set; }

    public string DispatcherName { get; set; } = string.Empty;

    public double CargoWeight { get; set; }

    public int CargoWidth { get; set; }

    public int CargoLength { get; set; }

    public int CargoHeight { get; set; }

    public ICollection<LoadStopDto> LoadStops { get; set; } = new List<LoadStopDto>();
}