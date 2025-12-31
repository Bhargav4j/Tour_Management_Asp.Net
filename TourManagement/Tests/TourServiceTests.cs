using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services.Tests;

/// <summary>
/// Unit tests for TourService
/// </summary>
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
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TourService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TourService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TourService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1" },
            new Tour { Id = 2, TourName = "Tour 2" }
        };
        var tourDtos = new List<TourDto>
        {
            new TourDto { Id = 1, TourName = "Tour 1" },
            new TourDto { Id = 2, TourName = "Tour 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "Test Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(tour);
        _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedTour()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "New Tour",
            Description = "Description",
            Place = "Paris",
            Price = 999.99m,
            Duration = 5
        };
        var createdTour = new Tour { Id = 1, TourName = "New Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default)).ReturnsAsync(createdTour);
        _mockMapper.Setup(m => m.Map<TourDto>(It.IsAny<Tour>())).Returns(tourDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Tour", result.TourName);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesTour()
    {
        // Arrange
        var updateDto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Description = "Updated Description",
            Price = 1500m
        };
        var existingTour = new Tour { Id = 1, TourName = "Old Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingTour);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Tour>(t => t.TourName == "Updated Tour"), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesTour()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingTours()
    {
        // Arrange
        var searchTerm = "paris";
        var tours = new List<Tour> { new Tour { Id = 1, Place = "Paris" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, Place = "Paris" } };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm, default)).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByPlaceAsync_WithPlace_ReturnsTours()
    {
        // Arrange
        var place = "Hawaii";
        var tours = new List<Tour> { new Tour { Id = 1, Place = place } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, Place = place } };

        _mockRepository.Setup(r => r.GetByPlaceAsync(place, default)).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetByPlaceAsync(place);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(place, result.First().Place);
    }

    [Fact]
    public async Task CreateAsync_SetsDefaultValues()
    {
        // Arrange
        var createDto = new TourCreateDto { TourName = "Test" };
        Tour capturedTour = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), default))
            .Callback<Tour, CancellationToken>((t, ct) => capturedTour = t)
            .ReturnsAsync((Tour t, CancellationToken ct) => t);
        _mockMapper.Setup(m => m.Map<TourDto>(It.IsAny<Tour>())).Returns(new TourDto());

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedTour);
        Assert.True(capturedTour.IsActive);
        Assert.Equal("System", capturedTour.CreatedBy);
    }
}
