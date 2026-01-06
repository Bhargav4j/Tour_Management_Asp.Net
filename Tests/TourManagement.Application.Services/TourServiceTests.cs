using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
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
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
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

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Name == "Tour 1");
        Assert.Contains(result, t => t.Name == "Tour 2");
    }

    [Fact]
    public async Task GetTourByIdAsync_ReturnsTour_WhenTourExists()
    {
        var tour = new Tour { Id = 1, Name = "Test Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _service.GetTourByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.Name);
    }

    [Fact]
    public async Task GetTourByIdAsync_ReturnsNull_WhenTourDoesNotExist()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var result = await _service.GetTourByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTourAsync_ThrowsArgumentNullException_WhenTourIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateTourAsync(null!));
    }

    [Fact]
    public async Task CreateTourAsync_SetsDates_AndReturnsCreatedTour()
    {
        var tour = new Tour { Name = "New Tour", Place = "Test Place" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _service.CreateTourAsync(tour);

        Assert.NotNull(result);
        Assert.True(tour.IsActive);
        Assert.NotEqual(default(DateTime), tour.CreatedDate);
    }

    [Fact]
    public async Task UpdateTourAsync_ThrowsArgumentNullException_WhenTourIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateTourAsync(1, null!));
    }

    [Fact]
    public async Task UpdateTourAsync_ThrowsInvalidOperationException_WhenTourDoesNotExist()
    {
        var tour = new Tour { Name = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateTourAsync(999, tour));
    }

    [Fact]
    public async Task DeleteTourAsync_ThrowsInvalidOperationException_WhenTourDoesNotExist()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteTourAsync(999));
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsMatchingTours()
    {
        var tours = new List<Tour> { new Tour { Id = 1, Name = "Beach Tour" } };
        _mockRepository.Setup(r => r.SearchAsync("Beach", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _service.SearchToursAsync("Beach");

        Assert.Single(result);
        Assert.Equal("Beach Tour", result.First().Name);
    }
}
