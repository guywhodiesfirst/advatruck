using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Driver
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    [Required]
    [MaxLength(15)]
    public string Phone { get; set; } = null!;
    [Required]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [MaxLength(500)]
    public string? Note { get; set; }
    public DateTime RegistrationDate { get; set; }
    //public Vehicle Vehicle { get; set; } = null!;
    public ICollection<DriverLocation> DriverLocations { get; set; } = null!;
}
