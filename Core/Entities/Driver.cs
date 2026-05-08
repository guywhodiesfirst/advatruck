namespace Core.Entities;

using System.ComponentModel.DataAnnotations;
using Core.Identity;

public class Driver
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public AppUser User { get; set; } = null!;

    [MaxLength(500)]
    public string? Note { get; set; }

    public ICollection<DriverLocation> DriverLocations { get; set; } = new List<DriverLocation>();

    public ICollection<Load> Loads { get; set; } = new List<Load>();
}