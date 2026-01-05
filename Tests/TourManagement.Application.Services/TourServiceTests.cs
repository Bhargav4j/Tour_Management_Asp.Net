using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TourService>> _loggerMock;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _repositoryMock = new Mock<ITourRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TourService>>();
        _service = new TourService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act & Assert
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTourDtos()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Tour 1" },
            new Tour { TourId = 2, TourName = "Tour 2" }
        };
        var tourDtos = new List<TourDto>
        {
            new TourDto { TourId = 1, TourName = "Tour 1" },
            new TourDto { TourId = 2, TourName = "Tour 2" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithNoTours_ShouldReturnEmptyList()
    {
        // Arrange
        var tours = new List<Tour>();
        var tourDtos = new List<TourDto>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTourDto()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Test Tour" };
        var tourDto = new TourDto { TourId = 1, TourName = "Test Tour" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TourId);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnTourDto()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m
        };
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m
        };
        var createdTour = new Tour
        {
            TourId = 1,
            TourName = "New Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m
        };
        var tourDto = new TourDto
        {
            TourId = 1,
            TourName = "New Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m
        };

        _mapperMock.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdTour);
        _mapperMock.Setup(m => m.Map<TourDto>(createdTour)).Returns(tourDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TourId);
        Assert.Equal("New Tour", result.TourName);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateTour()
    {
        // Arrange
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Rome",
            Days = 10,
            Price = 1500m,
            IsActive = true
        };
        var existingTour = new Tour
        {
            TourId = 1,
            TourName = "Original Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000m
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mapperMock.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Rome",
            Days = 10,
            Price = 1500m
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldStillCallRepository()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(999);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnTourDtos()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Beach Tour", Place = "Miami" },
            new Tour { TourId = 2, TourName = "Beach Holiday", Place = "Hawaii" }
        };
        var tourDtos = new List<TourDto>
        {
            new TourDto { TourId = 1, TourName = "Beach Tour", Place = "Miami" },
            new TourDto { TourId = 2, TourName = "Beach Holiday", Place = "Hawaii" }
        };

        _repositoryMock.Setup(r => r.SearchAsync("Beach", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _repositoryMock.Verify(r => r.SearchAsync("Beach", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var tours = new List<Tour>();
        var tourDtos = new List<TourDto>();

        _repositoryMock.Setup(r => r.SearchAsync("NonExistent", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ShouldCallRepository()
    {
        // Arrange
        var tours = new List<Tour>();
        var tourDtos = new List<TourDto>();

        _repositoryMock.Setup(r => r.SearchAsync("", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("");

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync("", It.IsAny<CancellationToken>()), Times.Once);
    }
}
