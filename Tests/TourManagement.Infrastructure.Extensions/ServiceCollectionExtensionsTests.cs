using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;

namespace Tests.TourManagement.Infrastructure.Extensions;

/// <summary>
/// Unit tests for ServiceCollectionExtensions
/// </summary>
public class ServiceCollectionExtensionsTests
{
    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=TestDb;"}
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
        var configuration = GetConfiguration();

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
        var configuration = GetConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var tourRepository = serviceProvider.GetService<ITourRepository>();
        Assert.NotNull(tourRepository);
        Assert.IsType<TourRepository>(tourRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersBookingRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var bookingRepository = serviceProvider.GetService<IBookingRepository>();
        Assert.NotNull(bookingRepository);
        Assert.IsType<BookingRepository>(bookingRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersUserRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetConfiguration();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var userRepository = serviceProvider.GetService<IUserRepository>();
        Assert.NotNull(userRepository);
        Assert.IsType<UserRepository>(userRepository);
    }

    [Fact]
    public void AddInfrastructureServices_RegistersRepositoriesAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetConfiguration();
        services.AddInfrastructureServices(configuration);

        // Act
        var tourRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITourRepository));
        var bookingRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IBookingRepository));
        var userRepoDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserRepository));

        // Assert
        Assert.NotNull(tourRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourRepoDescriptor.Lifetime);
        Assert.NotNull(bookingRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, bookingRepoDescriptor.Lifetime);
        Assert.NotNull(userRepoDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, userRepoDescriptor.Lifetime);
    }

    [Fact]
    public void AddInfrastructureServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetConfiguration();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.Same(services, result);
    }
}
