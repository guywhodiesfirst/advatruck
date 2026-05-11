namespace Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Bid
{
    public Guid Id { get; set; }

    public Guid AuctionLotId { get; set; }

    public AuctionLot AuctionLot { get; set; } = null!;

    public Guid DriverCreatedId { get; set; }

    public required Driver DriverCreated { get; set; }

    public decimal Rate { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}