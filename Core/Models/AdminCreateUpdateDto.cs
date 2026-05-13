namespace Core.Models;

using System.ComponentModel.DataAnnotations;

public class AdminCreateUpdateDto
{
    public Guid? Id { get; set; }

    [Required]
    public Guid UserId { get; set; }
}