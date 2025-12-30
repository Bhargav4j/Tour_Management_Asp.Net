using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Interfaces;
using TourManagement.Application.Services;

namespace TourManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // Register services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
