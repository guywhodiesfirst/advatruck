namespace Core.Models;

using Core.Enums;

public class AuctionLotDto
{
    public Guid Id { get; set; }

    public Guid LoadId { get; set; }

    public Guid DispatcherCreatedId { get; set; }

    public string DispatcherName { get; set; } = string.Empty;

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public AuctionStatus Status { get; set; }

    public string? Note { get; set; }

    public List<BidDto> Bids { get; set; } = new();
}