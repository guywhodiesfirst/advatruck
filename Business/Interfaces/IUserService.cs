namespace Business.Interfaces;


using Core.Models;

public interface IUserService
{
    /// <summary>
    /// Retrieves a list of users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of user profiles.</returns>
    Task<IEnumerable<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user with specified ID.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User profile.</returns>
    Task<UserProfileDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
}