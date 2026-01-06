using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Application.Services;

namespace Tests.TourManagement.Application.Extensions;

/// <summary>
/// Unit tests for ServiceCollectionExtensions
/// </summary>
public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersTourService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourService = serviceProvider.GetService<ITourService>();
        Assert.NotNull(tourService);
        Assert.IsType<TourService>(tourService);
    }

    [Fact]
    public void AddApplicationServices_RegistersBookingService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var bookingService = serviceProvider.GetService<IBookingService>();
        Assert.NotNull(bookingService);
        Assert.IsType<BookingService>(bookingService);
    }

    [Fact]
    public void AddApplicationServices_RegistersUserService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userService = serviceProvider.GetService<IUserService>();
        Assert.NotNull(userService);
        Assert.IsType<UserService>(userService);
    }

    [Fact]
    public void AddApplicationServices_RegistersServicesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddApplicationServices();

        // Act
        var tourServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourService));
        var bookingServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingService));
        var userServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserService));

        // Assert
        Assert.NotNull(tourServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourServiceDescriptor.Lifetime);
        Assert.NotNull(bookingServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, bookingServiceDescriptor.Lifetime);
        Assert.NotNull(userServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userServiceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplicationServices_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services
            .AddApplicationServices()
            .AddApplicationServices();

        // Assert
        Assert.NotNull(result);
    }
}
