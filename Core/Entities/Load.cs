namespace Core.Entities;

using System.ComponentModel.DataAnnotations;
using Core.Enums;

public class Load
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public LoadStatus LoadStatus { get; set; }

    public required ICollection<LoadStop> LoadStops { get; set; } = new List<LoadStop>();

    public DateTime? ClosedAt { get; set; }

    public decimal Rate { get; set; }

    public decimal? DriverCharge { get; set; }

    [StringLength(500)]
    public string Note { get; set; } = null!;

    public Guid? DriverId { get; set; }

    public Driver Driver { get; set; } = null!;

    public Guid DispatcherId { get; set; }

    public required Dispatcher Dispatcher { get; set; }

    public double CargoWeight { get; set; }

    public int CargoWidth { get; set; }

    public int CargoLength { get; set; }

    public int CargoHeight { get; set; }

    public ICollection<AuctionLot> AuctionLots { get; set; } = new List<AuctionLot>();
}