using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

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
    public async Task GetAllToursAsync_ReturnsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1500 },
            new Tour { Id = 2, TourName = "London Tour", Place = "London", Days = 3, Price = 1200 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);

        var result = await _service.GetAllToursAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tours);
    }

    [Fact]
    public async Task GetTourByIdAsync_ValidId_ReturnsTour()
    {
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1500 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(tour);

        var result = await _service.GetTourByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(tour);
    }

    [Fact]
    public async Task CreateTourAsync_ValidTour_CreatesTour()
    {
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1500 };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default)).ReturnsAsync(tour);

        var result = await _service.CreateTourAsync(tour);

        result.Should().NotBeNull();
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), default), Times.Once);
    }
}
