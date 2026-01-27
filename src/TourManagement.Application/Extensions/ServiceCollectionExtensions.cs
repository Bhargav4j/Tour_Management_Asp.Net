using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Services;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
