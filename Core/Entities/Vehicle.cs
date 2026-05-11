namespace Core.Entities;

using System.ComponentModel.DataAnnotations;

public class Vehicle
{
    public Guid Id { get; set; }

    [StringLength(255)]
    public required string Model { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 5, ErrorMessage = "License plate must be between 5 and 9 characters.")]
    [RegularExpression(
        @"^[A-Z0-9\- ]+$",
        ErrorMessage = "Only uppercase Latin letters, numbers, hyphens, and spaces are allowed.")]
    public string PlateNumber { get; set; } = string.Empty;

    public int CargoSpaceWidth { get; set; }

    public int CargoSpaceLength { get; set; }

    public int CargoSpaceHeight { get; set; }

    public int MaxWeight { get; set; }

    public Guid DriverId { get; set; }

    public Driver Driver { get; set; } = null!;
}
