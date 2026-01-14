using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Application.Tests;

/// <summary>
/// Test class for TourService
/// </summary>
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
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var service = new TourService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_WithTours_ReturnsMappedTourDtos()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Tour1" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Tour1" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tour1", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoTours_ReturnsEmptyList()
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
    public async Task GetAllAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tours = new List<Tour>();
        _repositoryMock.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(It.IsAny<IEnumerable<Tour>>())).Returns(new List<TourDto>());

        // Act
        await _service.GetAllAsync(cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMappedTourDto()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "Test Tour" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _repositoryMock.Setup(r => r.GetByIdAsync(1, cancellationToken)).ReturnsAsync((Tour?)null);

        // Act
        await _service.GetByIdAsync(1, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsCreatedTourDto()
    {
        // Arrange
        var createDto = new TourCreateDto { TourName = "New Tour" };
        var tour = new Tour { Id = 1, TourName = "New Tour" };
        var tourDto = new TourDto { Id = 1, TourName = "New Tour" };
        _mapperMock.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _repositoryMock.Setup(r => r.AddAsync(tour, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task CreateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var createDto = new TourCreateDto { TourName = "New Tour" };
        var tour = new Tour { TourName = "New Tour" };
        _mapperMock.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _repositoryMock.Setup(r => r.AddAsync(tour, cancellationToken)).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<TourDto>(tour)).Returns(new TourDto());

        // Act
        await _service.CreateAsync(createDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(tour, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var createDto = new TourCreateDto { TourName = "New Tour" };
        var tour = new Tour { TourName = "New Tour" };
        _mapperMock.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidIdAndDto_UpdatesTour()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Original Tour" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingTour);
        _mapperMock.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);
        _repositoryMock.Setup(r => r.UpdateAsync(existingTour, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        var existingTour = new Tour { Id = 1, TourName = "Original Tour" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, cancellationToken)).ReturnsAsync(existingTour);
        _mapperMock.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);
        _repositoryMock.Setup(r => r.UpdateAsync(existingTour, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(1, cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tour>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var updateDto = new TourUpdateDto { TourName = "Updated Tour" };
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_CallsRepositoryDelete()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _repositoryMock.Setup(r => r.DeleteAsync(1, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(1));
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMappedTourDtos()
    {
        // Arrange
        var tours = new List<Tour> { new Tour { Id = 1, TourName = "Grand Canyon" } };
        var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Grand Canyon" } };
        _repositoryMock.Setup(r => r.SearchAsync("Canyon", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours)).Returns(tourDtos);

        // Act
        var result = await _service.SearchAsync("Canyon");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Grand Canyon", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
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
    public async Task SearchAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var tours = new List<Tour>();
        _repositoryMock.Setup(r => r.SearchAsync("search", cancellationToken)).ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(It.IsAny<IEnumerable<Tour>>())).Returns(new List<TourDto>());

        // Act
        await _service.SearchAsync("search", cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.SearchAsync("search", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.SearchAsync("search"));
    }
}
