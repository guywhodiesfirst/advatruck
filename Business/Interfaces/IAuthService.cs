namespace Business.Interfaces;

using Core.Models;

public interface IAuthService
{
    /// <summary>
    /// Registrates a new user.
    /// </summary>
    /// <param name="dto">Registration request.</param>
    /// <returns>Auth response.</returns>
    Task<AuthResponseDto> RegisterAsync(RegistrationRequestDto dto);

    /// <summary>
    /// Logins a registered user.
    /// </summary>
    /// <param name="dto">Login request.</param>
    /// <returns>Auth response.</returns>
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
}