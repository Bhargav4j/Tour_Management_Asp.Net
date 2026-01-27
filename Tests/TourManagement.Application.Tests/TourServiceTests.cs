using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
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
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new TourService(_mockTourRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllToursAsync_WhenCalled_ShouldReturnAllTours()
    {
        // Arrange
        var expectedTours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1" },
            new Tour { Id = 2, TourName = "Tour 2" }
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
    public async Task GetAllToursAsync_WhenNoTours_ShouldReturnEmptyList()
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
        var expectedTour = new Tour { Id = 1, TourName = "Paris Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTour);

        // Act
        var result = await _tourService.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ShouldReturnNull()
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
    public async Task GetTourByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.GetTourByIdAsync(1));
    }

    [Fact]
    public async Task CreateTourAsync_WithValidTour_ShouldReturnCreatedTour()
    {
        // Arrange
        var newTour = new Tour { TourName = "New Tour", Place = "Rome" };
        var createdTour = new Tour { Id = 1, TourName = "New Tour", Place = "Rome" };

        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _tourService.CreateTourAsync(newTour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(newTour.IsActive);
        Assert.NotEqual(default(DateTime), newTour.CreatedDate);
        _mockTourRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldSetCreatedDateAndIsActive()
    {
        // Arrange
        var newTour = new Tour { TourName = "Test Tour" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTour);

        // Act
        await _tourService.CreateTourAsync(newTour);

        // Assert
        Assert.True(newTour.IsActive);
        Assert.NotEqual(default(DateTime), newTour.CreatedDate);
    }

    [Fact]
    public async Task CreateTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var newTour = new Tour { TourName = "Test Tour" };
        _mockTourRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.CreateTourAsync(newTour));
    }

    [Fact]
    public async Task UpdateTourAsync_WithExistingTour_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Old Name" };
        var updatedTour = new Tour { Id = 1, TourName = "New Name" };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(updatedTour);

        // Assert
        Assert.NotEqual(default(DateTime), updatedTour.ModifiedDate);
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockTourRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var updatedTour = new Tour { Id = 999, TourName = "Non-Existing" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tourService.UpdateTourAsync(updatedTour));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateTourAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Tour" };
        var updatedTour = new Tour { Id = 1, TourName = "Updated Tour" };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.UpdateTourAsync(updatedTour);

        // Assert
        Assert.NotNull(updatedTour.ModifiedDate);
        Assert.NotEqual(default(DateTime), updatedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingTour_ShouldDeleteSuccessfully()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Tour to Delete" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _tourService.DeleteTourAsync(1);

        // Assert
        _mockTourRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockTourRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tourService.DeleteTourAsync(999));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteTourAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Tour" };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockTourRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.DeleteTourAsync(1));
    }

    [Fact]
    public async Task SearchToursAsync_WithSearchTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var searchTerm = "Paris";
        var expectedTours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Paris Adventure" }
        };
        _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTours);

        // Act
        var result = await _tourService.SearchToursAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockTourRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_WithNoMatches_ShouldReturnEmptyList()
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

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockTourRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _tourService.SearchToursAsync("test"));
    }

    [Fact]
    public async Task GetAllToursAsync_WithCancellationToken_ShouldPassToken()
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
}
