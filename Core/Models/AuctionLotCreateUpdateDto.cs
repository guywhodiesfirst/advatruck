namespace Core.Models;

public class AuctionLotCreateUpdateDto
{
    public Guid LoadId { get; set; }

    public DateTime EndsAt { get; set; }

    public Guid DispatcherCreatedId { get; set; }

    public string? Note { get; set; }
}