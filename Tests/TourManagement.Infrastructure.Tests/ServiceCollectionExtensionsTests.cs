using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Infrastructure.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_ShouldRegisterDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;User Id=sa;Password=Test123;" }
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<TourManagementDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterTourRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;User Id=sa;Password=Test123;" }
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetService<ITourRepository>();
        Assert.NotNull(repository);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterUserRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;User Id=sa;Password=Test123;" }
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetService<IUserRepository>();
        Assert.NotNull(repository);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterBookingRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;User Id=sa;Password=Test123;" }
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var repository = serviceProvider.GetService<IBookingRepository>();
        Assert.NotNull(repository);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;User Id=sa;Password=Test123;" }
            })
            .Build();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructureServices_WithNullConfiguration_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddInfrastructureServices(null!));
    }
}
