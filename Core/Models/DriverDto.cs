namespace Core.Models;

public class DriverDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? VehiclePlate { get; set; }

    public string? Note { get; set; }
}