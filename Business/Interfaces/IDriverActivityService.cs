namespace Business.Interfaces;

/// <summary>
/// Monitor responsible for tracking drivers and detecting inactivity.
/// </summary>
public interface IDriverActivityService
{
    /// <summary>
    /// Check if any of the drivers should update their location.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<List<Guid>> GetInactiveDriversAsync(CancellationToken cancellationToken);
}