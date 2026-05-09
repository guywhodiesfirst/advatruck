namespace Core.Models;

public class DispatcherDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Note { get; set; }
}