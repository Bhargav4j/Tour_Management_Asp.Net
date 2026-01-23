using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TourManagement.Infrastructure.Extensions;
using TourManagement.Domain.Interfaces.Repositories;
using System.Collections.Generic;

namespace TourManagement.Infrastructure.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_RegistersAllRepositories()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Integrated Security=true;" }
            })
            .Build();

        // Act
        services.AddInfrastructureServices(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IUserRepository>());
        Assert.NotNull(serviceProvider.GetService<ITourRepository>());
        Assert.NotNull(serviceProvider.GetService<IBookingRepository>());
    }
}
