using Xunit;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Extensions;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Tests.Application.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_RegistersAllServices()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<ITourService>());
        Assert.NotNull(serviceProvider.GetService<IUserService>());
        Assert.NotNull(serviceProvider.GetService<IBookingService>());
    }

    [Fact]
    public void AddApplicationServices_RegistersServicesAsScoped()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddApplicationServices();

        var tourServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITourService));
        Assert.NotNull(tourServiceDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, tourServiceDescriptor.Lifetime);
    }
}
