namespace Core.Models;

public class UserProfileDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string? Role { get; set; }
}