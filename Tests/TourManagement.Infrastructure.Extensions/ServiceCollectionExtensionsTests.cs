using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    private IConfiguration CreateConfiguration()
    {
        var configurationDict = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configurationDict)
            .Build();
    }

    [Fact]
    public void AddInfrastructureServices_RegistersDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<TourManagementDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersAllRepositories()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourRepository = serviceProvider.GetService<ITourRepository>();
        var userRepository = serviceProvider.GetService<IUserRepository>();
        var bookingRepository = serviceProvider.GetService<IBookingRepository>();

        Assert.NotNull(tourRepository);
        Assert.NotNull(userRepository);
        Assert.NotNull(bookingRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersTourRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourRepository));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(TourRepository), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersUserRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserRepository));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(UserRepository), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersBookingRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingRepository));
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(BookingRepository), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructureServices_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CreateConfiguration();

        // Act & Assert
        var result = services
            .AddInfrastructureServices(configuration)
            .AddLogging();

        Assert.NotNull(result);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersDbContextWithCorrectConnectionString()
    {
        // Arrange
        var services = new ServiceCollection();
        var expectedConnectionString = "Server=testserver;Database=testdb;";
        var configurationDict = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", expectedConnectionString}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationDict)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<TourManagementDbContext>();
        Assert.NotNull(dbContext);
    }
}
