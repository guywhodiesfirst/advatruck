namespace Business.Services;

using Business.Interfaces;
using Core.Entities;
using Core.Types;

public class DistanceService : IDistanceService
{
    private const double EarthRadiusKm = 6371.0;

    /// <inheritdoc />
    public double GetDistanceInKm(GeoPoint point1, GeoPoint point2)
    {
        var dLat = ToRadians(point2.Latitude - point1.Latitude);
        var dLon = ToRadians(point2.Longitude - point1.Longitude);

        var lat1 = ToRadians(point1.Latitude);
        var lat2 = ToRadians(point2.Latitude);

        var a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) +
                (Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2));

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = EarthRadiusKm * c;

        return Math.Round(distance, 2);
    }

    /// <inheritdoc />
    public double CalculateRouteDistanceInKm(IEnumerable<LoadStop>? stops)
    {
        if (stops == null)
        {
            return 0;
        }

        var orderedStops = stops.OrderBy(s => s.Timestamp).ToList();
        if (orderedStops.Count < 2)
        {
            return 0;
        }

        double totalDistance = 0;

        for (int i = 0; i < orderedStops.Count - 1; i++)
        {
            totalDistance += GetDistanceInKm(orderedStops[i].Location, orderedStops[i + 1].Location);
        }

        return Math.Round(totalDistance, 2);
    }

    private static double ToRadians(double angleIn10)
    {
        return (Math.PI / 180) * angleIn10;
    }
}