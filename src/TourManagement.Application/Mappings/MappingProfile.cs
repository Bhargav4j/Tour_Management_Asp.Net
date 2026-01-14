using AutoMapper;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Tour, TourDto>();
        CreateMap<TourCreateDto, Tour>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<TourUpdateDto, Tour>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UserInfo, UserDto>();
        CreateMap<UserCreateDto, UserInfo>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<UserUpdateDto, UserInfo>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<Booking, BookingDto>();
        CreateMap<BookingCreateDto, Booking>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        CreateMap<BookingUpdateDto, Booking>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
