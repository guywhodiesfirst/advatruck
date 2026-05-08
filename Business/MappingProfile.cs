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
    }
}