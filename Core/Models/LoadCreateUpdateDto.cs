namespace Core.Models;

using System.ComponentModel.DataAnnotations;
using Core.Enums;

public class LoadCreateUpdateDto
{
    public LoadStatus LoadStatus { get; set; }

    public string? Note { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
    public decimal Rate { get; set; }

    [Required]
    public Guid DispatcherId { get; set; }

    [Required]
    [Range(0.1, 100000)]
    public double CargoWeight { get; set; }

    [Required]
    public int CargoWidth { get; set; }

    [Required]
    public int CargoLength { get; set; }

    [Required]
    public int CargoHeight { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "A load must have at least two stops (pickup and delivery).")]
    public ICollection<LoadStopDto> LoadStops { get; set; } = new List<LoadStopDto>();
}