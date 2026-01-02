using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

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
    public async Task GetAllAsync_ShouldReturnTourDtos()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Tour1" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Tour1" } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour1", result.First().TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTourDto_WhenTourExists()
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
    public async Task GetByIdAsync_ShouldReturnNull_WhenTourDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTour_AndReturnTourDto()
    {
        // Arrange
        var createDto = new TourCreateDto { TourName = "New Tour" };
        var tour = new Tour { TourName = "New Tour" };
        var createdTour = new Tour { Id = 1, TourName = "New Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };

        _mockMapper.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _mockRepository.Setup(r => r.AddAsync(tour, It.IsAny<CancellationToken>())).ReturnsAsync(createdTour);
        _mockMapper.Setup(m => m.Map<TourDto>(createdTour)).Returns(tourDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTour_WhenTourExists()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Old Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mockMapper.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(existingTour, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenTourDoesNotExist()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteTour()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTours()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Paris Tour" } };

        _mockRepository.Setup(r => r.SearchAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Tour", result.First().TourName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(_mockRepository.Object, _mockMapper.Object, null!));
    }
}
