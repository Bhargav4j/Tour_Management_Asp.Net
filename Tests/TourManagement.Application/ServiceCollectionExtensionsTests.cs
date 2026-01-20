using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersTourService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var tourServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourService));
        Assert.NotNull(tourServiceDescriptor);
        Assert.Equal(typeof(TourService), tourServiceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_RegistersUserInfoService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var userInfoServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserInfoService));
        Assert.NotNull(userInfoServiceDescriptor);
        Assert.Equal(typeof(UserInfoService), userInfoServiceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_RegistersBookingService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var bookingServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingService));
        Assert.NotNull(bookingServiceDescriptor);
        Assert.Equal(typeof(BookingService), bookingServiceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_RegistersAllServicesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var tourServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourService));
        var userInfoServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserInfoService));
        var bookingServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingService));

        Assert.NotNull(tourServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourServiceDescriptor.Lifetime);

        Assert.NotNull(userInfoServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userInfoServiceDescriptor.Lifetime);

        Assert.NotNull(bookingServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, bookingServiceDescriptor.Lifetime);
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
            .AddSingleton<string>("test");

        // Assert
        Assert.Same(services, result);
        Assert.Contains(services, s => s.ServiceType == typeof(ITourService));
        Assert.Contains(services, s => s.ServiceType == typeof(string));
    }
}
