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
            new Tour { Id = 1, Name = "Tour 1", IsActive = true },
            new Tour { Id = 2, Name = "Tour 2", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);

        var result = await _service.GetAllToursAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tours);
    }

    [Fact]
    public async Task CreateTourAsync_SetsDatesAndActiveStatus()
    {
        var tour = new Tour { Name = "New Tour", CreatedBy = "admin" };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken ct) => t);

        var result = await _service.CreateTourAsync(tour);

        result.IsActive.Should().BeTrue();
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
