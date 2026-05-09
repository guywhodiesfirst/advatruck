namespace Business;

using AutoMapper;
using Core.Entities;
using Core.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VehicleCreateUpdateDto, Vehicle>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.Empty));

        CreateMap<Vehicle, VehicleCreateUpdateDto>();

        CreateMap<Vehicle, VehicleDto>()
            .ForMember(
                dest => dest.DriverName,
                opt => opt.MapFrom(
                    src => src.Driver.User.FirstName + " " + src.Driver.User.LastName));

        CreateMap<LoadStopDto, LoadStop>()
            .ReverseMap();

        CreateMap<LoadCreateUpdateDto, Load>()
            .ForMember(dest => dest.LoadStops, opt => opt.MapFrom(src => src.LoadStops));

        CreateMap<Load, LoadCreateUpdateDto>();

        CreateMap<Load, LoadDto>()
            .ForMember(
                dest => dest.DispatcherName,
                opt => opt.MapFrom(src => src.Dispatcher.User.FirstName + " " + src.Dispatcher.User.LastName))
            .ForMember(
                dest => dest.DriverName,
                opt => opt.MapFrom(src => src.Driver.User.FirstName + " " + src.Driver.User.LastName));
    }
}