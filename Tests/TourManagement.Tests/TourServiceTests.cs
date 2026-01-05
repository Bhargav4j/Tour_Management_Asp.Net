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
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new TourService(_mockTourRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockTourRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnAllTours()
    {
        // Arrange
        var expectedTours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Paris Tour" },
            new Tour { TourId = 2, TourName = "Rome Tour" }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTours);

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllToursAsync_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _mockTourRepository.Setup(r => r.GetAllAsync(cancellationToken))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _tourService.GetAllToursAsync(cancellationToken);

        // Assert
        _mockTourRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tourId = 1;
        var expectedTour = new Tour { TourId = tourId, TourName = "Paris Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTour);

        // Act
        var result = await _tourService.GetTourByIdAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tourId, result.TourId);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var tourId = 999;
        _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetTourByIdAsync(tourId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithZeroId_ShouldCallRepository()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(0, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetTourByIdAsync(0);

        // Assert
        Assert.Null(result);
        _mockTourRepository.Verify(r => r.GetByIdAsync(0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetTourByIdAsync(1));
    }

    [Fact]
    public async Task CreateTourAsync_WithValidTour_ShouldCreateAndReturnTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Place = "London" };
        var createdTour = new Tour { TourId = 1, TourName = "New Tour", Place = "London" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TourId);
        Assert.True(tour.IsActive);
        Assert.NotEqual(default(DateTime), tour.CreatedDate);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour" };
        var beforeCreate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        await _tourService.CreateTourAsync(tour);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(tour.CreatedDate >= beforeCreate);
        Assert.True(tour.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", IsActive = false };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public async Task CreateTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_WithExistingTour_ShouldUpdateTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Updated Tour" };
        var existingTour = new Tour { TourId = 1, TourName = "Old Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(tour);

        // Assert
        Assert.NotEqual(default(DateTime), tour.ModifiedDate);
        _mockTourRepository.Verify(r => r.UpdateAsync(tour, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_ShouldSetModifiedDateToUtcNow()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Updated Tour" };
        var existingTour = new Tour { TourId = 1 };
        var beforeUpdate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(tour);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(tour.ModifiedDate);
        Assert.True(tour.ModifiedDate >= beforeUpdate);
        Assert.True(tour.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { TourId = 999, TourName = "Non-existing Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Updated Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingTourId_ShouldDeleteTour()
    {
        // Arrange
        var tourId = 1;
        _mockTourRepository.Setup(r => r.ExistsAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.DeleteAsync(tourId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.DeleteTourAsync(tourId);

        // Assert
        _mockTourRepository.Verify(r => r.DeleteAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistingTourId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tourId = 999;
        _mockTourRepository.Setup(r => r.ExistsAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tourService.DeleteTourAsync(tourId));
    }

    [Fact]
    public async Task DeleteTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var tourId = 1;
        _mockTourRepository.Setup(r => r.ExistsAsync(tourId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.DeleteTourAsync(tourId));
    }

    [Fact]
    public async Task SearchToursAsync_WithValidSearchTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var searchTerm = "Paris";
        var expectedTours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Paris Adventure" },
            new Tour { TourId = 2, TourName = "Paris Night Tour" }
        };
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTours);

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchToursAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        var searchTerm = "NonExisting";
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchToursAsync_WithEmptySearchTerm_ShouldCallRepository()
    {
        // Arrange
        var searchTerm = "";
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _tourService.SearchToursAsync(searchTerm);

        // Assert
        _mockTourRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var searchTerm = "Paris";
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.SearchToursAsync(searchTerm));
    }
}
