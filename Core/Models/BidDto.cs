namespace Core.Models;

public class BidDto
{
    public Guid Id { get; set; }

    public Guid DriverCreatedId { get; set; }

    public string DriverName { get; set; } = null!;

    public decimal Rate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Note { get; set; }

    public string? LastLocationAddress { get; set; }

    public DateTime? LastLocationUpdate { get; set; }
}