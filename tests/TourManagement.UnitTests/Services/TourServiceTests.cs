using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 1000 },
            new Tour { TourId = 2, TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 1500 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().AllBeOfType<TourDto>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTour_WhenTourExists()
    {
        var tour = new Tour { TourId = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 1000 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.TourId.Should().Be(1);
        result.TourName.Should().Be("Tour 1");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTour()
    {
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 5,
            Price = 1000,
            Locations = "Location 1",
            TourInfo = "Info"
        };

        var tour = new Tour { TourId = 1, TourName = "New Tour" };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        var result = await _service.CreateAsync(createDto);

        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
