using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

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
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1" },
            new Tour { Id = 2, TourName = "Tour 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidTour_ShouldReturnCreatedTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Place = "Paris" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken ct) =>
            {
                t.Id = 1;
                return t;
            });

        // Act
        var result = await _service.CreateAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdateTour()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Old Name" };
        var updatedTour = new Tour { TourName = "New Name", Place = "Paris", Days = 5, Price = 1000 };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedTour);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var updatedTour = new Tour { TourName = "New Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, updatedTour));
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), default), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteTour()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.ExistsAsync(1, default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
        _mockRepository.Verify(r => r.ExistsAsync(999, default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), default), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour" },
            new Tour { Id = 2, TourName = "Paris Adventure" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Paris", default)).ReturnsAsync(tours);

        // Act
        var result = await _service.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync("Paris", default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ShouldReturnResults()
    {
        // Arrange
        var tours = new List<Tour>();
        _mockRepository.Setup(r => r.SearchAsync(string.Empty, default)).ReturnsAsync(tours);

        // Act
        var result = await _service.SearchAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.SearchAsync(string.Empty, default), Times.Once);
    }
}
