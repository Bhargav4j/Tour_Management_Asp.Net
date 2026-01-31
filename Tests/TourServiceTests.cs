using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

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
    public void Constructor_WithNullTourRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockTourRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Rome Tour" }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        var result = await _tourService.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetTourByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockTourRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_WithValidTour_CreatesTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Place = "Paris", Days = 7, Price = 1999m };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        var result = await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        _mockTourRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_WithNullTour_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _tourService.CreateTourAsync(null!));
    }

    [Fact]
    public async Task UpdateTourAsync_WithValidTour_UpdatesTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        var result = await _tourService.UpdateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ModifiedDate);
        _mockTourRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNullTour_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _tourService.UpdateTourAsync(null!));
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistentTour_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { Id = 999, TourName = "Nonexistent Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingTour_ReturnsTrue()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _tourService.DeleteTourAsync(1);

        // Assert
        Assert.True(result);
        _mockTourRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistentTour_ReturnsFalse()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _tourService.DeleteTourAsync(999);

        // Assert
        Assert.False(result);
        _mockTourRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SearchToursAsync_WithSearchTerm_ReturnsMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Paris Adventure" }
        };
        _mockTourRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        // Act
        var result = await _tourService.SearchToursAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_WithException_ThrowsException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_WithException_ThrowsException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetTourByIdAsync(1));
    }

    [Fact]
    public async Task CreateTourAsync_WithException_ThrowsException()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateTourAsync(tour));
    }
}
