namespace Business.Interfaces;
using Core.Identity;

/// <summary>
/// Service for managing JWT tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a JWT token.
    /// </summary>
    /// <param name="user">User.</param>
    /// <param name="role">Role.</param>
    /// <returns>JWT token.</returns>
    Task<string> CreateToken(AppUser user, string role);
}