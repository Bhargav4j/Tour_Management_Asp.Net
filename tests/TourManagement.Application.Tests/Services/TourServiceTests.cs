using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests.Services;

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
    public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockTourRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" },
            new Tour { Id = 2, TourName = "Rome Tour", Place = "Rome" }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _tourService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _tourService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockTourRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNewTour_ShouldCreateAndReturnTour()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "New York",
            Days = 5,
            Price = 1000m
        };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _tourService.CreateAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        _mockTourRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateAsync(tour));
    }

    [Fact]
    public async Task UpdateAsync_WithExistingTour_ShouldUpdateTour()
    {
        // Arrange
        var existingTour = new Tour
        {
            Id = 1,
            TourName = "Old Tour",
            Place = "Old Place"
        };
        var updatedTour = new Tour
        {
            TourName = "Updated Tour",
            Place = "New Place",
            Days = 7,
            Price = 2000m,
            Locations = "Location1, Location2",
            TourInfo = "Updated info",
            PicturePath = "/images/updated.jpg"
        };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateAsync(1, updatedTour);

        // Assert
        Assert.Equal("Updated Tour", existingTour.TourName);
        Assert.Equal("New Place", existingTour.Place);
        Assert.Equal(7, existingTour.Days);
        Assert.Equal(2000m, existingTour.Price);
        Assert.NotNull(existingTour.ModifiedDate);
        Assert.Equal("System", existingTour.ModifiedBy);
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockTourRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { TourName = "Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _tourService.UpdateAsync(999, tour));
        Assert.Contains("not found", exception.Message);
        _mockTourRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingTour_ShouldDeleteTour()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.DeleteAsync(1);

        // Assert
        _mockTourRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockTourRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _tourService.DeleteAsync(999));
        Assert.Contains("not found", exception.Message);
        _mockTourRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" },
            new Tour { Id = 2, TourName = "Paris Adventure", Place = "Paris" }
        };
        _mockTourRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _tourService.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _tourService.SearchAsync("xyz");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockTourRepository.Verify(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.SearchAsync("test", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.SearchAsync("test"));
    }
}
