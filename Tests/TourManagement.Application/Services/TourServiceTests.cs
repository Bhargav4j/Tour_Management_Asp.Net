using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Services;

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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1", Place = "Hawaii" },
            new Tour { Id = 2, TourName = "Tour 2", Place = "Alps" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllToursAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _service.GetAllToursAsync(cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Hawaii" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task GetTourByIdAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetByIdAsync(1, cts.Token))
            .ReturnsAsync((Tour?)null);

        // Act
        await _service.GetTourByIdAsync(1, cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, cts.Token), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_WithValidData_ShouldCreateTour()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", Place = "Europe", Days = 10, Price = 2000.00m };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour t, CancellationToken ct) => { t.Id = 1; return t; });

        // Act
        var result = await _service.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTourAsync_WithNullTour_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateTourAsync(null!));
    }

    [Fact]
    public async Task CreateTourAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var tour = new Tour { TourName = "Test", Place = "Test" };
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), cts.Token))
            .ReturnsAsync((Tour t, CancellationToken ct) => { t.Id = 1; return t; });

        // Act
        await _service.CreateTourAsync(tour, cts.Token);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithValidData_ShouldUpdateTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Updated Tour", Place = "Updated Place" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Tour { Id = 1, TourName = "Original Tour" });
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.UpdateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Tour", result.TourName);
        Assert.NotNull(result.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNullTour_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateTourAsync(null!));
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistentTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { Id = 999, TourName = "Nonexistent" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test" };
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetByIdAsync(1, cts.Token))
            .ReturnsAsync(new Tour { Id = 1 });
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), cts.Token))
            .ReturnsAsync(tour);

        // Act
        await _service.UpdateTourAsync(tour, cts.Token);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), cts.Token), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteTourAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteTourAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTourAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.ExistsAsync(1, cts.Token))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, cts.Token))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteTourAsync(1, cts.Token);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, cts.Token), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Beach Paradise", Place = "Hawaii" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Beach", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.SearchToursAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockRepository.Verify(r => r.SearchAsync("Beach", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchToursAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("NotFound", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _service.SearchToursAsync("NotFound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchToursAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.SearchAsync("test", cts.Token))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _service.SearchToursAsync("test", cts.Token);

        // Assert
        _mockRepository.Verify(r => r.SearchAsync("test", cts.Token), Times.Once);
    }
}
