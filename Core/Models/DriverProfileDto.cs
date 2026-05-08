namespace Core.Models;

using Core.Types;

/// <summary>
/// Data transfer object representing driver profile information.
/// </summary>
public class DriverProfileDto : ProfileDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime RegistrationDate { get; set; }

    public string? PhoneNumber { get; set; }

    public GeoPoint? LastLocation { get; set; }
}