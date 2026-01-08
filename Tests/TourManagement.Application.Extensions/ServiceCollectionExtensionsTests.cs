using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourService = serviceProvider.GetService<ITourService>();
        var userService = serviceProvider.GetService<IUserService>();
        var bookingService = serviceProvider.GetService<IBookingService>();

        Assert.NotNull(tourService);
        Assert.NotNull(userService);
        Assert.NotNull(bookingService);
    }

    [Fact]
    public void AddApplicationServices_RegistersTourServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(TourService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_RegistersUserServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(UserService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_RegistersBookingServiceAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingService));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(BookingService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddApplicationServices_ReturnsServiceCollection()
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
    public void AddApplicationServices_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var result = services
            .AddApplicationServices()
            .AddApplicationServices();

        Assert.NotNull(result);
    }
}
