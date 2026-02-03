using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Application.Services;

namespace TourManagement.Application.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterTourService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplicationServices();
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));

        // Assert
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(TourService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterUserInfoService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplicationServices();
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserInfoService));

        // Assert
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(UserInfoService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterBookingService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplicationServices();
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));

        // Assert
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(BookingService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterAllServicesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var tourServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        var userInfoServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserInfoService));
        var bookingServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));

        Assert.NotNull(tourServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourServiceDescriptor.Lifetime);

        Assert.NotNull(userInfoServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userInfoServiceDescriptor.Lifetime);

        Assert.NotNull(bookingServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, bookingServiceDescriptor.Lifetime);
    }
}
