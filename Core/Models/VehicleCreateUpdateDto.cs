namespace Core.Models;

using System.ComponentModel.DataAnnotations;

public class VehicleCreateUpdateDto
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(255)]
    public required string Model { get; set; }

    [Required]
    [StringLength(9, MinimumLength = 5, ErrorMessage = "License plate must be between 5 and 9 characters.")]
    [RegularExpression(
        @"^[A-Z0-9\- ]+$",
        ErrorMessage = "Only uppercase Latin letters, numbers, hyphens, and spaces are allowed.")]
    public required string PlateNumber { get; set; }

    public int CargoSpaceWidth { get; set; }

    public int CargoSpaceLength { get; set; }

    public int CargoSpaceHeight { get; set; }

    public int MaxWeight { get; set; }

    [Required(ErrorMessage = "Driver must be assigned to the vehicle.")]
    public Guid DriverId { get; set; }
}