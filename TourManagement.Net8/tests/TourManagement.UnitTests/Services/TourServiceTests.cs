using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.UnitTests.Services;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<TourService>>();
        _service = new TourService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 },
            new Tour { Id = 2, TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200 }
        };

        var tourDtos = new List<TourDto>
        {
            new TourDto { Id = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 },
            new TourDto { Id = 2, TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
            .Returns(tourDtos);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tourDtos);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTour()
    {
        var tour = new Tour { Id = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };
        var tourDto = new TourDto { Id = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour))
            .Returns(tourDto);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(tourDto);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }
}
