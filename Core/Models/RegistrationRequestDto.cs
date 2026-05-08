namespace Core.Models;

public record RegistrationRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role);