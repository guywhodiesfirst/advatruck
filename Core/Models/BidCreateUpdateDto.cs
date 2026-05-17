namespace Core.Models;

public class BidCreateUpdateDto
{
    public Guid AuctionLotId { get; set; }

    public Guid DriverCreatedId { get; set; }

    public decimal Rate { get; set; }

    public string? Note { get; set; }
}