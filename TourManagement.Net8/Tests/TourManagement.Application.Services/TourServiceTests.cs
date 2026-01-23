using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Application.Services.Tests;

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
    public async Task GetAllAsync_ReturnsTourDtos()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Paris Tour" } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTourDto_WhenTourExists()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "Paris Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTourDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTourDto()
    {
        // Arrange
        var createDto = new TourCreateDto { TourName = "New Tour" };
        var tour = new Tour { Id = 1, TourName = "New Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };

        _mockMapper.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _mockRepository.Setup(r => r.AddAsync(tour, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenTourDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Paris Tour" } };

        _mockRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByPlaceAsync_ReturnsToursForPlace()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, Place = "Paris" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, Place = "Paris" } };

        _mockRepository.Setup(r => r.GetByPlaceAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetByPlaceAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
