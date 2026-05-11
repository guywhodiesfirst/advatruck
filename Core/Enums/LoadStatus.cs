namespace Core.Enums;

/// <summary>
/// Represents the status of the load.
/// </summary>
public enum LoadStatus
{
    /// <summary>
    /// Scheduled load.
    /// </summary>
    Scheduled,

    /// <summary>
    /// Ongoing load.
    /// </summary>
    Ongoing,

    /// <summary>
    /// Completed load.
    /// </summary>
    Completed,

    /// <summary>
    /// Cancelled load.
    /// </summary>
    Cancelled,
}