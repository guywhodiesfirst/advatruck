namespace Core.Models;

using System.ComponentModel.DataAnnotations;
using Core.Enums;
using Core.Types;

public class LoadStopDto
{
    [Required]
    public LoadLocationType LoadLocationType { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public string? Note { get; set; }

    [Required]
    public GeoPoint Location { get; set; } = null!;
}