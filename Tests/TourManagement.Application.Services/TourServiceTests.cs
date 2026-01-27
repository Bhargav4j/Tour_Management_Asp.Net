using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

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

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public void TourService_Constructor_CreatesInstance()
    {
        // Arrange & Act
        var service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedTourDtos()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { Id = 2, TourName = "Rome Tour", Place = "Rome", Days = 6, Price = 1200m, IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.TourName == "Paris Tour");
        Assert.Contains(result, t => t.TourName == "Rome Tour");
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTourDto()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesTourAndReturnsTourDto()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 7,
            Price = 1500m,
            Locations = "Location1, Location2",
            TourInfo = "Great tour"
        };

        var createdTour = new Tour
        {
            Id = 1,
            TourName = "New Tour",
            Place = "New Place",
            Days = 7,
            Price = 1500m,
            Locations = "Location1, Location2",
            TourInfo = "Great tour",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Tour", result.TourName);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesTour()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Original Tour", Place = "Original Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Days = 6,
            Price = 1200m,
            Locations = "Updated Locations",
            TourInfo = "Updated Info"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Adventure", Place = "Paris", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { Id = 2, TourName = "Paris Nights", Place = "Paris", Days = 3, Price = 800m, IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Contains("Paris", t.TourName));
        _mockRepository.Verify(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("NoMatch", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Tour>());

        // Act
        var result = await _service.SearchAsync("NoMatch");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _service.GetAllAsync(cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsPropertiesCorrectly()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000m,
            Locations = "Loc1, Loc2",
            TourInfo = "Info",
            PicturePath = "/images/test.jpg"
        };

        Tour capturedTour = null!;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Callback<Tour, CancellationToken>((tour, ct) => capturedTour = tour)
            .ReturnsAsync((Tour tour, CancellationToken ct) => tour);

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedTour);
        Assert.Equal("Test Tour", capturedTour.TourName);
        Assert.Equal("Test Place", capturedTour.Place);
        Assert.Equal(5, capturedTour.Days);
        Assert.Equal(1000m, capturedTour.Price);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var existingTour = new Tour { Id = 1, TourName = "Original", Place = "Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new TourUpdateDto { TourName = "Updated", Place = "Updated Place", Days = 6, Price = 1200m };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTour);

        Tour capturedTour = null!;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .Callback<Tour, CancellationToken>((tour, ct) => capturedTour = tour)
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(capturedTour);
        Assert.NotNull(capturedTour.ModifiedDate);
    }
}
