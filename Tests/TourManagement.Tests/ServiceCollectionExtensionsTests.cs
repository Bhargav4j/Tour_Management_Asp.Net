using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IServiceCollection>(result);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterTourService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(TourService), serviceDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterUserService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(UserService), serviceDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterBookingService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(typeof(BookingService), serviceDescriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterAllThreeServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        Assert.Equal(3, services.Count);
        Assert.Contains(services, s => s.ServiceType == typeof(ITourService));
        Assert.Contains(services, s => s.ServiceType == typeof(IUserService));
        Assert.Contains(services, s => s.ServiceType == typeof(IBookingService));
    }

    [Fact]
    public void AddApplicationServices_WithExistingServices_ShouldAddApplicationServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<string>("Test Service");

        // Act
        services.AddApplicationServices();

        // Assert
        Assert.Equal(4, services.Count);
        Assert.Contains(services, s => s.ServiceType == typeof(ITourService));
        Assert.Contains(services, s => s.ServiceType == typeof(IUserService));
        Assert.Contains(services, s => s.ServiceType == typeof(IBookingService));
        Assert.Contains(services, s => s.ServiceType == typeof(string));
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
        Assert.Equal(6, services.Count);
    }

    [Fact]
    public void AddApplicationServices_WithEmptyServiceCollection_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.Same(services, result);
        Assert.NotEmpty(services);
    }

    [Fact]
    public void AddApplicationServices_RegisteredServices_ShouldHaveScopedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        Assert.All(services, s => Assert.Equal(ServiceLifetime.Scoped, s.Lifetime));
    }
}
