using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Tests;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_ServicesAreRegistered()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        // Assert - Verify repositories are registered
        var tourRepository = services.GetService<ITourRepository>();
        var userRepository = services.GetService<IUserRepository>();
        var bookingRepository = services.GetService<IBookingRepository>();

        Assert.NotNull(tourRepository);
        Assert.NotNull(userRepository);
        Assert.NotNull(bookingRepository);
    }

    [Fact]
    public void Program_ServicesLayerIsRegistered()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        // Assert - Verify services are registered
        var tourService = services.GetService<ITourService>();
        var userService = services.GetService<IUserService>();
        var bookingService = services.GetService<IBookingService>();

        Assert.NotNull(tourService);
        Assert.NotNull(userService);
        Assert.NotNull(bookingService);
    }

    [Fact]
    public async Task Program_ApplicationStarts()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void Program_HttpContextAccessorIsRegistered()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        // Assert
        var httpContextAccessor = services.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        Assert.NotNull(httpContextAccessor);
    }
}
