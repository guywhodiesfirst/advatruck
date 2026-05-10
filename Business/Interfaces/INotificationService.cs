namespace Business.Interfaces;

/// <summary>
/// Provides a mechanism for publishing driver and load-related events to external notification systems.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Publishes an event indicating that a driver has become inactive during an ongoing load.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishDriverInactiveAsync(Guid driverId);

    /// <summary>
    /// Publishes an event when a new load is assigned to a driver.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver.</param>
    /// <param name="loadId">The unique identifier of the assigned load.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishLoadAssignedAsync(Guid driverId, Guid loadId);

    /// <summary>
    /// Publishes an event when an assigned load is canceled.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver.</param>
    /// <param name="loadId">The unique identifier of the canceled load.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PublishLoadCanceledAsync(Guid driverId, Guid loadId);
}