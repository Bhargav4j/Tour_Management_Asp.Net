using AutoMapper;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Tour, TourDto>();

        CreateMap<TourCreateDto, Tour>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());

        CreateMap<TourUpdateDto, Tour>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());

        CreateMap<User, UserDto>();

        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());

        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());

        CreateMap<Booking, BookingDto>();

        CreateMap<BookingCreateDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Tour, opt => opt.Ignore());

        CreateMap<BookingUpdateDto, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TourId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Tour, opt => opt.Ignore());
    }
}
