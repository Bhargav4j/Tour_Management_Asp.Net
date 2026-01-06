using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

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
        // Arrange
        var expectedTours = new List<Tour>
        {
            new Tour { Id = 1, Name = "Paris Tour", Place = "Paris", Days = 5, Price = 1500 },
            new Tour { Id = 2, Name = "London Tour", Place = "London", Days = 3, Price = 1200 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTours);

        // Act
        var result = await _service.GetAllToursAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedTours);
    }

    [Fact]
    public async Task GetTourByIdAsync_WhenTourExists_ShouldReturnTour()
    {
        // Arrange
        var expectedTour = new Tour { Id = 1, Name = "Paris Tour", Place = "Paris", Days = 5, Price = 1500 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTour);

        // Act
        var result = await _service.GetTourByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedTour);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldCreateTour()
    {
        // Arrange
        var newTour = new Tour { Name = "New Tour", Place = "New Place", Days = 7, Price = 2000 };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken ct) => { t.Id = 1; return t; });

        // Act
        var result = await _service.CreateTourAsync(newTour);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
