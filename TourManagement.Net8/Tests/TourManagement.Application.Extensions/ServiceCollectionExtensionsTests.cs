using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Application.Interfaces.Services;

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
        Assert.NotNull(serviceProvider.GetService<IUserService>());
        Assert.NotNull(serviceProvider.GetService<ITourService>());
        Assert.NotNull(serviceProvider.GetService<IBookingService>());
    }
}
