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
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _tourService;

    public TourServiceTests()
    {
        _mockTourRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _tourService = new TourService(_mockTourRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Tour 1", Price = 100 },
            new Tour { TourId = 2, TourName = "Tour 2", Price = 200 }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tours);
    }

    [Fact]
    public async Task GetTourByIdAsync_ExistingId_ReturnsTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Test Tour", Price = 100 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _tourService.GetTourByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(tour);
    }

    [Fact]
    public async Task CreateTourAsync_ValidTour_ReturnsTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Price = 150, CreatedBy = "Test" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken ct) => t);

        // Act
        var result = await _tourService.CreateTourAsync(tour);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        result.IsActive.Should().BeTrue();
    }
}
