namespace Core.Entities;

using System.ComponentModel.DataAnnotations;
using Core.Enums;

public class AuctionLot
{
    public Guid Id { get; set; }

    public Guid LoadId { get; set; }

    public Guid DispatcherCreatedId { get; set; }

    public required Dispatcher DispatcherCreated { get; set; }

    public required Load Load { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public AuctionStatus Status { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}