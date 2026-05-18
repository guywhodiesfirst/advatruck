namespace Business.Interfaces;

using Core.Entities;
using Core.Types;

public interface IDistanceService
{
    /// <summary>
    /// Retrieves approximate distance between two locations.
    /// </summary>
    /// <param name="point1">First coordinate..</param>
    /// <param name="point2">Second coordinate.</param>
    /// <returns>Distance in kilometers.</returns>
    double GetDistanceInKm(GeoPoint point1, GeoPoint point2);

    /// <summary>
    /// Retrieves the full distance for a set of LoadStops.
    /// </summary>
    /// <param name="stops">Set of stops.</param>
    /// <returns>Distance in kilometers.</returns>
    double CalculateRouteDistanceInKm(IEnumerable<LoadStop> stops);
}