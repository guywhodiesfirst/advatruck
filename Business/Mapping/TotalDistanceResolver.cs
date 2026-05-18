namespace Business.Mapping;

using AutoMapper;
using Business.Interfaces;
using Core.Entities;
using Core.Models;

public class TotalDistanceResolver(IDistanceService distanceService) : IValueResolver<Load, LoadDto, double>
{
    public double Resolve(Load source, LoadDto destination, double destMember, ResolutionContext context)
    {
        if (source.LoadStops.Count == 0)
        {
            return 0;
        }

        return distanceService.CalculateRouteDistanceInKm(source.LoadStops);
    }
}