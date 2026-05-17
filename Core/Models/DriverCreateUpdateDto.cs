namespace Core.Models;

using System.ComponentModel.DataAnnotations;

public class DriverCreateUpdateDto
{
    [Required]
    public Guid UserId { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
}