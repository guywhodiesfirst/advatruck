namespace Business;

using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Identity;
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
        CreateMap<DriverCreateUpdateDto, Driver>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.NewGuid()));

        CreateMap<Driver, DriverDto>()
            .ForMember(
                dest => dest.UserId,
                opt => opt.MapFrom(src => src.UserId))
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(
                dest => dest.VehiclePlate,
                opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.PlateNumber : null));

        CreateMap<DispatcherCreateUpdateDto, Dispatcher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.NewGuid()));

        CreateMap<Dispatcher, DispatcherDto>()
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<AdminCreateUpdateDto, Admin>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? Guid.NewGuid()));

        CreateMap<Admin, AdminDto>()
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<AppUser, UserProfileDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.RegistrationDate));

        CreateMap<Dispatcher, DispatcherProfileDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => src.User.RegistrationDate));

        CreateMap<AuctionLot, AuctionLotDto>()
            .ForMember(dest => dest.DispatcherName, opt => opt.MapFrom(src => src.DispatcherCreated.User.FirstName + " " + src.DispatcherCreated.User.LastName))
            .ForMember(dest => dest.Bids, opt => opt.MapFrom(src => src.Bids.OrderByDescending(b => b.CreatedAt)));

        CreateMap<AuctionLotCreateUpdateDto, AuctionLot>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Bids, opt => opt.Ignore())
            .ForMember(dest => dest.DispatcherCreated, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AuctionStatus.Active));

        CreateMap<Bid, BidDto>()
            .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.DriverCreated.User.FirstName + " " + src.DriverCreated.User.LastName));

        CreateMap<BidCreateUpdateDto, Bid>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.DriverCreated, opt => opt.Ignore())
            .ForMember(dest => dest.AuctionLot, opt => opt.Ignore());
    }
}