using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockTourRepository.Object, null));
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
        _mockTourRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        // Act
        var result = await _tourService.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockTourRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        var tourId = 1;
        var tour = new Tour { Id = tourId, TourName = "Paris Tour" };
        _mockTourRepository.Setup(x => x.GetByIdAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        // Act
        var result = await _tourService.GetTourByIdAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tourId, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockTourRepository.Verify(x => x.GetByIdAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var tourId = 999;
        _mockTourRepository.Setup(x => x.GetByIdAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _tourService.GetTourByIdAsync(tourId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockTourRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetTourByIdAsync(1));
    }

    [Fact]
    public async Task CreateTourAsync_WithValidTour_ReturnsTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris" };
        var createdTour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", CreatedDate = DateTime.UtcNow };
        _mockTourRepository.Setup(x => x.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdTour);

        // Act
        var result = await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(tour.IsActive);
        Assert.NotEqual(default(DateTime), tour.CreatedDate);
        _mockTourRepository.Verify(x => x.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_SetsCreatedDateAndIsActive()
    {
        // Arrange
        var tour = new Tour { TourName = "Paris Tour" };
        var createdTour = new Tour { Id = 1, TourName = "Paris Tour", CreatedDate = DateTime.UtcNow, IsActive = true };
        _mockTourRepository.Setup(x => x.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdTour);

        // Act
        await _tourService.CreateTourAsync(tour);

        // Assert
        Assert.NotEqual(default(DateTime), tour.CreatedDate);
        Assert.True(tour.IsActive);
    }

    [Fact]
    public async Task CreateTourAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var tour = new Tour { TourName = "Paris Tour" };
        _mockTourRepository.Setup(x => x.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_WithExistingTour_UpdatesSuccessfully()
    {
        // Arrange
        var tourId = 1;
        var existingTour = new Tour { Id = tourId, TourName = "Old Name" };
        var updatedTour = new Tour { Id = tourId, TourName = "New Name" };
        _mockTourRepository.Setup(x => x.GetByIdAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockTourRepository.Setup(x => x.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(updatedTour);

        // Assert
        Assert.NotEqual(default(DateTime), updatedTour.ModifiedDate);
        _mockTourRepository.Verify(x => x.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistingTour_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { Id = 999, TourName = "New Name" };
        _mockTourRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_SetsModifiedDate()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Old Name" };
        _mockTourRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockTourRepository.Setup(x => x.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(tour);

        // Assert
        Assert.NotNull(tour.ModifiedDate);
        Assert.NotEqual(default(DateTime), tour.ModifiedDate);
    }

    [Fact]
    public async Task UpdateTourAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Old Name" };
        _mockTourRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockTourRepository.Setup(x => x.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingId_DeletesSuccessfully()
    {
        // Arrange
        var tourId = 1;
        _mockTourRepository.Setup(x => x.ExistsAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockTourRepository.Setup(x => x.DeleteAsync(tourId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _tourService.DeleteTourAsync(tourId);

        // Assert
        _mockTourRepository.Verify(x => x.DeleteAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistingId_ThrowsInvalidOperationException()
    {
        // Arrange
        var tourId = 999;
        _mockTourRepository.Setup(x => x.ExistsAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _tourService.DeleteTourAsync(tourId));
    }

    [Fact]
    public async Task DeleteTourAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var tourId = 1;
        _mockTourRepository.Setup(x => x.ExistsAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockTourRepository.Setup(x => x.DeleteAsync(tourId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.DeleteTourAsync(tourId));
    }

    [Fact]
    public async Task SearchToursAsync_WithSearchTerm_ReturnsTours()
    {
        // Arrange
        var searchTerm = "Paris";
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" },
            new Tour { Id = 2, TourName = "Paris Adventure", Place = "Paris" }
        };
        _mockTourRepository.Setup(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_WithEmptySearchTerm_ReturnsAllTours()
    {
        // Arrange
        var searchTerm = string.Empty;
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Rome Tour" }
        };
        _mockTourRepository.Setup(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var searchTerm = "Paris";
        _mockTourRepository.Setup(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.SearchToursAsync(searchTerm));
    }
}
