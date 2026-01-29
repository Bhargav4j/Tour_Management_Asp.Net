using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Tests.Application.Services;

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
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, Name = "Tour 1" },
            new Tour { Id = 2, Name = "Tour 2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _service.GetAllToursAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllToursAsync_WhenRepositoryThrows_ThrowsTourManagementException()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<TourManagementException>(() => _service.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ReturnsTour()
    {
        var tour = new Tour { Id = 1, Name = "Test Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _service.GetTourByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetTourByIdAsync(999));
    }

    [Fact]
    public async Task CreateTourAsync_WithValidTour_ReturnsTour()
    {
        var tour = new Tour { Name = "New Tour", Place = "Paris" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _service.CreateTourAsync(tour);

        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(DateTime.MinValue, result.CreatedDate);
    }

    [Fact]
    public async Task CreateTourAsync_SetsCreatedDateAndIsActive()
    {
        var tour = new Tour { Name = "New Tour" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        await _service.CreateTourAsync(tour);

        Assert.True(tour.IsActive);
        Assert.NotEqual(DateTime.MinValue, tour.CreatedDate);
    }

    [Fact]
    public async Task UpdateTourAsync_WithExistingTour_UpdatesTour()
    {
        var existingTour = new Tour { Id = 1, Name = "Old Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var updatedTour = new Tour { Id = 1, Name = "Updated Tour" };
        await _service.UpdateTourAsync(updatedTour);

        Assert.NotNull(updatedTour.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTourAsync_WithNonExistingTour_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var tour = new Tour { Id = 999, Name = "Nonexistent" };
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task DeleteTourAsync_WithExistingId_DeletesTour()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.DeleteTourAsync(1);

        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTourAsync_WithNonExistingId_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteTourAsync(999));
    }

    [Fact]
    public async Task SearchToursAsync_WithSearchTerm_ReturnsMatchingTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, Name = "Paris Tour" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _service.SearchToursAsync("Paris");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchToursAsync_WhenRepositoryThrows_ThrowsTourManagementException()
    {
        _mockRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Search error"));

        await Assert.ThrowsAsync<TourManagementException>(() => _service.SearchToursAsync("test"));
    }
}
