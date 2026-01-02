using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;
using Xunit;

namespace TourManagement.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterTourService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourService = serviceProvider.GetService<ITourService>();
        Assert.NotNull(tourService);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterUserService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userService = serviceProvider.GetService<IUserService>();
        Assert.NotNull(userService);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterBookingService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var bookingService = serviceProvider.GetService<IBookingService>();
        Assert.NotNull(bookingService);
    }

    [Fact]
    public void AddApplicationServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationServices();

        // Assert
        Assert.Same(services, result);
    }
}
