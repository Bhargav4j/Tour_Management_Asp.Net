using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;
using System.Collections.Generic;

namespace TourManagement.Infrastructure.Tests;

public class InfrastructureServiceCollectionExtensionsTests
{
    private IConfiguration GetTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public void AddInfrastructureServices_RegistersDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<TourManagementDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersTourRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = GetTestConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourRepository = serviceProvider.GetService<ITourRepository>();
        Assert.NotNull(tourRepository);
        Assert.IsType<TourRepository>(tourRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersUserInfoRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = GetTestConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userInfoRepository = serviceProvider.GetService<IUserInfoRepository>();
        Assert.NotNull(userInfoRepository);
        Assert.IsType<UserInfoRepository>(userInfoRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersBookingRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = GetTestConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var bookingRepository = serviceProvider.GetService<IBookingRepository>();
        Assert.NotNull(bookingRepository);
        Assert.IsType<BookingRepository>(bookingRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersAllRepositoriesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var tourRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourRepository));
        var userInfoRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserInfoRepository));
        var bookingRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingRepository));

        Assert.NotNull(tourRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourRepoDescriptor.Lifetime);

        Assert.NotNull(userInfoRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userInfoRepoDescriptor.Lifetime);

        Assert.NotNull(bookingRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, bookingRepoDescriptor.Lifetime);
    }

    [Fact]
    public void AddInfrastructureServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructureServices_CanBeChained()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetTestConfiguration();

        // Act
        var result = services
            .AddInfrastructureServices(configuration)
            .AddSingleton<string>("test");

        // Assert
        Assert.Same(services, result);
        Assert.Contains(services, s => s.ServiceType == typeof(ITourRepository));
        Assert.Contains(services, s => s.ServiceType == typeof(string));
    }
}
