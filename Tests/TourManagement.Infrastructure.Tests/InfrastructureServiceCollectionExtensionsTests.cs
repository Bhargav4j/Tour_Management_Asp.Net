using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Tests;

public class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_WithValidConfiguration_ShouldRegisterAllServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;Trusted_Connection=True;" }
            }!)
            .Build();

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
    public void AddInfrastructureServices_WithMissingConnectionString_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddInfrastructureServices(configuration));
        Assert.Contains("Connection string 'DefaultConnection' not found", exception.Message);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterTourRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(TourRepository), descriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterUserRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(UserRepository), descriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterBookingRepositoryAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingRepository));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(BookingRepository), descriptor.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TourManagementDbContext));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        var result = services.AddInfrastructureServices(configuration);

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructureServices_WithConnectionString_ShouldConfigureDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var connectionString = "Server=testserver;Database=testdb;User Id=testuser;Password=testpass;";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", connectionString }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TourManagementDbContext));
        Assert.NotNull(dbContextDescriptor);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldRegisterCorrectImplementationType()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);

        // Assert
        var tourDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourRepository));
        var userDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserRepository));
        var bookingDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IBookingRepository));

        Assert.Equal(typeof(TourRepository), tourDescriptor?.ImplementationType);
        Assert.Equal(typeof(UserRepository), userDescriptor?.ImplementationType);
        Assert.Equal(typeof(BookingRepository), bookingDescriptor?.ImplementationType);
    }

    [Fact]
    public void AddInfrastructureServices_ShouldAllowChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=Test;" }
            }!)
            .Build();

        // Act
        var result = services
            .AddInfrastructureServices(configuration);

        // Assert
        Assert.NotNull(result);
        Assert.True(services.Count > 0);
    }
}
