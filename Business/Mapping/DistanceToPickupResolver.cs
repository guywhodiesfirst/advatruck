namespace Business.Mapping;

using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Models;

public class DistanceToPickupResolver(IDistanceService distanceService) : IValueResolver<Bid, BidDto, double>
{
    public double Resolve(Bid source, BidDto destination, double destMember, ResolutionContext context)
    {
        if (source.DriverCreated?.DriverLocations == null)
        {
            return 0;
        }

        if (source.AuctionLot?.Load?.LoadStops == null)
        {
            return 0;
        }

        var lastDriverLocation = source.DriverCreated.DriverLocations
            .OrderByDescending(l => l.UpdateTime)
            .FirstOrDefault();

        var firstPickupStop = source.AuctionLot.Load.LoadStops
            .OrderBy(s => s.Timestamp)
            .FirstOrDefault();

        if (lastDriverLocation?.Location == null || firstPickupStop?.Location == null)
        {
            return 0;
        }

        return distanceService.GetDistanceInKm(lastDriverLocation.Location, firstPickupStop.Location);
    }
}