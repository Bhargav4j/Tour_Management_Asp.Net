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
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Tour 1", Place = "Place 1" },
            new Tour { TourId = 2, TourName = "Tour 2", Place = "Place 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tours);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTour_WhenExists()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Test Tour", Place = "Test Place" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TourId.Should().Be(1);
        result.TourName.Should().Be("Test Tour");
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTour_WithCorrectProperties()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 5,
            Price = 1000
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.CreateAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
