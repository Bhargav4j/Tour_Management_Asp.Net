using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;
using FluentAssertions;

namespace TourManagement.UnitTests.Services;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnAllTours()
    {
        var expectedTours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Price = 1000 },
            new Tour { Id = 2, TourName = "London Tour", Price = 1500 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTours);

        var result = await _service.GetAllToursAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedTours);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldSetCreatedDateAndIsActive()
    {
        var newTour = new Tour
        {
            TourName = "New Tour",
            Price = 2000
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken ct) => t);

        var result = await _service.CreateTourAsync(newTour);

        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.IsActive.Should().BeTrue();
    }
}
