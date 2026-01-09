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
    public void TourService_Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var service = new TourService(_mockRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
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
    public async Task GetAllToursAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetTourByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetTourByIdAsync(1));
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

    [Fact]
    public async Task CreateTourAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", IsActive = false };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        await _service.CreateTourAsync(tour);

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public async Task CreateTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_WithExistingTour_ShouldUpdateTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Old Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateTourAsync(tour);

        // Assert
        Assert.NotEqual(default(DateTime), tour.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { Id = 999, TourName = "Updated Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateTourAsync(tour));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingTour_ShouldDeleteTour()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteTourAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteTourAsync(999));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteTourAsync(1));
    }

    [Fact]
    public async Task SearchToursAsync_WithValidSearchTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var searchTerm = "Grand";
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Grand Canyon" },
            new Tour { Id = 2, TourName = "Grand Tour of Europe" }
        };
        _mockRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SearchToursAsync("test"));
    }
}
