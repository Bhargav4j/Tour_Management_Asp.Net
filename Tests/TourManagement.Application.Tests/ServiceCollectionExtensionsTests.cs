using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Application.Services;

namespace TourManagement.Application.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var descriptor1 = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        var descriptor2 = services.FirstOrDefault(d => d.ServiceType == typeof(IUserService));
        var descriptor3 = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));

        Assert.NotNull(descriptor1);
        Assert.NotNull(descriptor2);
        Assert.NotNull(descriptor3);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterTourServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(TourService), descriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterUserServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(UserService), descriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterBookingServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(BookingService), descriptor.ImplementationType);
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
    public void AddApplicationServices_CalledMultipleTimes_ShouldRegisterServicesMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        services.AddApplicationServices();

        // Assert
        var tourServiceDescriptors = services.Where(d => d.ServiceType == typeof(ITourService)).ToList();
        Assert.Equal(2, tourServiceDescriptors.Count);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterCorrectImplementationType()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var tourDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        var userDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserService));
        var bookingDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));

        Assert.Equal(typeof(TourService), tourDescriptor?.ImplementationType);
        Assert.Equal(typeof(UserService), userDescriptor?.ImplementationType);
        Assert.Equal(typeof(BookingService), bookingDescriptor?.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_WithEmptyServiceCollection_ShouldAddThreeServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        Assert.Equal(3, services.Count);
    }

    [Fact]
    public void AddApplicationServices_ShouldAllowChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services
            .AddApplicationServices()
            .AddApplicationServices();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, services.Count);
    }
}
