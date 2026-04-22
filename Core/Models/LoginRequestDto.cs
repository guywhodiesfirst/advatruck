namespace Core.Models;

public record LoginRequestDto
{
    public string Email { get; set; } = null!;
}