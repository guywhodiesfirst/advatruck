namespace Business.Interfaces;

using Core.Models;

/// <summary>
/// Service for managing driver business logic.
/// </summary>
public interface IDriverService
{
    /// <summary>
    /// Retrieve all drivers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of drivers.</returns>
    Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get driver by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver if found, otherwise throws an exception.</returns>
    Task<DriverDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new driver.
    /// </summary>
    /// <param name="dto">Driver data transfer object for creation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>ID of created driver.</returns>
    Task<Guid> CreateAsync(DriverCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update driver information.
    /// </summary>
    /// <param name="dto">Driver with updated data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated driver information.</returns>
    Task<DriverDto> UpdateAsync(DriverCreateUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete driver by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task representing async operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full driver profile information by email.
    /// </summary>
    /// <param name="email">Driver's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver profile data transfer object.</returns>
    Task<DriverProfileDto?> GetProfileByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves full driver profile information by ID.
    /// </summary>
    /// <param name="id">Driver's ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Driver profile data transfer object.</returns>
    Task<DriverProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default);
}