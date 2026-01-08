using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _bookingService = new BookingService(_mockBookingRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullBookingRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, null));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourName = "Paris Tour", Email = "user1@example.com" },
            new Booking { Id = 2, TourName = "Rome Tour", Email = "user2@example.com" }
        };
        _mockBookingRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockBookingRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetAllBookingsAsync());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        var bookingId = 1;
        var booking = new Booking { Id = bookingId, TourName = "Paris Tour" };
        _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookingId, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
        _mockBookingRepository.Verify(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var bookingId = 999;
        _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockBookingRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingByIdAsync(1));
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithValidUserId_ReturnsBookings()
    {
        // Arrange
        var userId = 1;
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = userId, TourName = "Paris Tour" },
            new Booking { Id = 2, UserId = userId, TourName = "Rome Tour" }
        };
        _mockBookingRepository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithInvalidUserId_ReturnsEmpty()
    {
        // Arrange
        var userId = 999;
        _mockBookingRepository.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockBookingRepository.Setup(x => x.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingsByUserIdAsync(1));
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithValidTourId_ReturnsBookings()
    {
        // Arrange
        var tourId = 1;
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = tourId, TourName = "Paris Tour" },
            new Booking { Id = 2, TourId = tourId, TourName = "Paris Tour" }
        };
        _mockBookingRepository.Setup(x => x.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(x => x.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithInvalidTourId_ReturnsEmpty()
    {
        // Arrange
        var tourId = 999;
        _mockBookingRepository.Setup(x => x.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockBookingRepository.Setup(x => x.GetByTourIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingsByTourIdAsync(1));
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidBooking_ReturnsBooking()
    {
        // Arrange
        var booking = new Booking { TourName = "Paris Tour", Email = "test@example.com" };
        var createdBooking = new Booking { Id = 1, TourName = "Paris Tour", Email = "test@example.com" };
        _mockBookingRepository.Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(booking.IsActive);
        Assert.NotEqual(default(DateTime), booking.CreatedDate);
        Assert.NotEqual(default(DateTime), booking.BookingDate);
        _mockBookingRepository.Verify(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsCreatedDateBookingDateAndIsActive()
    {
        // Arrange
        var booking = new Booking { TourName = "Paris Tour" };
        var createdBooking = new Booking { Id = 1, TourName = "Paris Tour" };
        _mockBookingRepository.Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotEqual(default(DateTime), booking.CreatedDate);
        Assert.NotEqual(default(DateTime), booking.BookingDate);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var booking = new Booking { TourName = "Paris Tour" };
        _mockBookingRepository.Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithExistingBooking_UpdatesSuccessfully()
    {
        // Arrange
        var bookingId = 1;
        var existingBooking = new Booking { Id = bookingId, TourName = "Old Tour" };
        var updatedBooking = new Booking { Id = bookingId, TourName = "New Tour" };
        _mockBookingRepository.Setup(x => x.GetByIdAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(x => x.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(updatedBooking);

        // Assert
        Assert.NotEqual(default(DateTime), updatedBooking.ModifiedDate);
        _mockBookingRepository.Verify(x => x.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistingBooking_ThrowsInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { Id = 999, TourName = "New Tour" };
        _mockBookingRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_SetsModifiedDate()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourName = "Updated Tour" };
        var existingBooking = new Booking { Id = 1, TourName = "Old Tour" };
        _mockBookingRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(x => x.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(booking);

        // Assert
        Assert.NotNull(booking.ModifiedDate);
        Assert.NotEqual(default(DateTime), booking.ModifiedDate);
    }

    [Fact]
    public async Task UpdateBookingAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourName = "Updated Tour" };
        var existingBooking = new Booking { Id = 1, TourName = "Old Tour" };
        _mockBookingRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(x => x.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingId_DeletesSuccessfully()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository.Setup(x => x.ExistsAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepository.Setup(x => x.DeleteAsync(bookingId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteBookingAsync(bookingId);

        // Assert
        _mockBookingRepository.Verify(x => x.DeleteAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistingId_ThrowsInvalidOperationException()
    {
        // Arrange
        var bookingId = 999;
        _mockBookingRepository.Setup(x => x.ExistsAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.DeleteBookingAsync(bookingId));
    }

    [Fact]
    public async Task DeleteBookingAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository.Setup(x => x.ExistsAsync(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepository.Setup(x => x.DeleteAsync(bookingId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.DeleteBookingAsync(bookingId));
    }
}
