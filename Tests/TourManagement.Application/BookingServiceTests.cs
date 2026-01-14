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
/// Test class for BookingService
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<BookingService>> _loggerMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _repositoryMock = new Mock<IBookingRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var service = new BookingService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_WithBookings_ReturnsMappedBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, TourName = "Tour1" } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, TourName = "Tour1" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tour1", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        var bookings = new List<Booking>();
        var bookingDtos = new List<BookingDto>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

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
        var bookings = new List<Booking>();
        _repositoryMock.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(It.IsAny<IEnumerable<Booking>>())).Returns(new List<BookingDto>());

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
    public async Task GetByIdAsync_WithValidId_ReturnsMappedBookingDto()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourName = "Test Booking" };
        var bookingDto = new BookingDto { Id = 1, TourName = "Test Booking" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Booking", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

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
        _repositoryMock.Setup(r => r.GetByIdAsync(1, cancellationToken)).ReturnsAsync((Booking?)null);

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
    public async Task GetByUserEmailAsync_WithValidEmail_ReturnsMappedBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, Email = "test@example.com", TourName = "Booking1" } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, Email = "test@example.com", TourName = "Booking1" } };
        _repositoryMock.Setup(r => r.GetByUserEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("test@example.com", result.First().Email);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        var bookings = new List<Booking>();
        var bookingDtos = new List<BookingDto>();
        _repositoryMock.Setup(r => r.GetByUserEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var bookings = new List<Booking>();
        _repositoryMock.Setup(r => r.GetByUserEmailAsync("test@example.com", cancellationToken)).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(It.IsAny<IEnumerable<Booking>>())).Returns(new List<BookingDto>());

        // Act
        await _service.GetByUserEmailAsync("test@example.com", cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByUserEmailAsync("test@example.com", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByUserEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetByUserEmailAsync("test@example.com"));
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsCreatedBookingDto()
    {
        // Arrange
        var createDto = new BookingCreateDto { TourName = "New Booking", Email = "test@example.com" };
        var booking = new Booking { Id = 1, TourName = "New Booking", Email = "test@example.com" };
        var bookingDto = new BookingDto { Id = 1, TourName = "New Booking", Email = "test@example.com" };
        _mapperMock.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _repositoryMock.Setup(r => r.AddAsync(booking, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Booking", result.TourName);
    }

    [Fact]
    public async Task CreateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var createDto = new BookingCreateDto { TourName = "New Booking" };
        var booking = new Booking { TourName = "New Booking" };
        _mapperMock.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _repositoryMock.Setup(r => r.AddAsync(booking, cancellationToken)).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(new BookingDto());

        // Act
        await _service.CreateAsync(createDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(booking, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var createDto = new BookingCreateDto { TourName = "New Booking" };
        var booking = new Booking { TourName = "New Booking" };
        _mapperMock.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidIdAndDto_UpdatesBooking()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { BookingDate = DateTime.UtcNow };
        var existingBooking = new Booking { Id = 1, TourName = "Original Booking" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mapperMock.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);
        _repositoryMock.Setup(r => r.UpdateAsync(existingBooking, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { BookingDate = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var updateDto = new BookingUpdateDto { BookingDate = DateTime.UtcNow };
        var existingBooking = new Booking { Id = 1, TourName = "Original Booking" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, cancellationToken)).ReturnsAsync(existingBooking);
        _mapperMock.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);
        _repositoryMock.Setup(r => r.UpdateAsync(existingBooking, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByIdAsync(1, cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { BookingDate = DateTime.UtcNow };
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
}
