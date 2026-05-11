namespace Core.Models;

public record LoginRequestDto
{
    public string Email { get; set; } = null!;

    public required string Password { get; set; }
}