using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests;

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
    public void TourService_Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void TourService_Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
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
            new Tour { Id = 1, TourName = "Tour 1" },
            new Tour { Id = 2, TourName = "Tour 2" }
        };
        _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsEmptyList_WhenNoToursExist()
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
    public async Task GetTourByIdAsync_ReturnsTour_WhenTourExists()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _tourService.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetTourByIdAsync_ReturnsNull_WhenTourDoesNotExist()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetTourByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTourAsync_CreatesAndReturnsTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Price = 1000m };
        var createdTour = new Tour { Id = 1, TourName = "New Tour", Price = 1000m };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Tour", result.TourName);
        Assert.True(tour.IsActive);
        _mockTourRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_SetsCreatedDate()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour" };
        var beforeCreate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        await _tourService.CreateTourAsync(tour);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(tour.CreatedDate >= beforeCreate && tour.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateTourAsync_SetsIsActiveToTrue()
    {
        // Arrange
        var tour = new Tour { TourName = "Test Tour", IsActive = false };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public async Task UpdateTourAsync_UpdatesTour_WhenTourExists()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Old Tour" };
        var updatedTour = new Tour { Id = 1, TourName = "Updated Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(updatedTour);

        // Assert
        Assert.NotNull(updatedTour.ModifiedDate);
        _mockTourRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_ThrowsEntityNotFoundException_WhenTourDoesNotExist()
    {
        // Arrange
        var tour = new Tour { Id = 999, TourName = "Non-existent Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_SetsModifiedDate()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Tour" };
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
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
        Assert.True(tour.ModifiedDate >= beforeUpdate && tour.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task DeleteTourAsync_DeletesTour_WhenTourExists()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Tour to Delete" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.DeleteTourAsync(1);

        // Assert
        _mockTourRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_ThrowsEntityNotFoundException_WhenTourDoesNotExist()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _tourService.DeleteTourAsync(999));
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsMatchingTours()
    {
        // Arrange
        var searchTerm = "Paris";
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Paris Adventure" }
        };
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsEmptyList_WhenNoMatchFound()
    {
        // Arrange
        var searchTerm = "NonExistent";
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
