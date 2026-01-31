using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Services;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockTourRepository = new Mock<ITourRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullBookingRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            null!,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            _mockBookingRepository.Object,
            null!,
            _mockUserRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            null!,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 2, UserId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockBookingRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<Booking>());

        // Act
        await _service.GetAllBookingsAsync(cts.Token);

        // Assert
        _mockBookingRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1, NumberOfPeople = 2 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _service.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(2, result.NumberOfPeople);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetBookingByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 100 },
            new Booking { Id = 2, TourId = 2, UserId = 100 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsByUserIdAsync(100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(100, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(1, cts.Token))
            .ReturnsAsync(new List<Booking>());

        // Act
        await _service.GetBookingsByUserIdAsync(1, cts.Token);

        // Assert
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(1, cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 50, UserId = 1 },
            new Booking { Id = 2, TourId = 50, UserId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsByTourIdAsync(50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(1, cts.Token))
            .ReturnsAsync(new List<Booking>());

        // Act
        await _service.GetBookingsByTourIdAsync(1, cts.Token);

        // Assert
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(1, cts.Token), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ShouldCreateBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 3 };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Price = 500.00m };
        var user = new User { Id = 1, Username = "testuser" };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking b, CancellationToken ct) => { b.Id = 1; return b; });

        // Act
        var result = await _service.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1500.00m, result.TotalAmount);
        Assert.True(result.IsActive);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNullBooking_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateBookingAsync(null!));
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidTourId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 999, UserId = 1, NumberOfPeople = 2 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidUserId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 1, UserId = 999, NumberOfPeople = 2 };
        var tour = new Tour { Id = 1, Price = 500.00m };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCalculateTotalAmount()
    {
        // Arrange
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 4 };
        var tour = new Tour { Id = 1, Price = 250.00m };
        var user = new User { Id = 1, Username = "testuser" };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking b, CancellationToken ct) => { b.Id = 1; return b; });

        // Act
        var result = await _service.CreateBookingAsync(booking);

        // Assert
        Assert.Equal(1000.00m, result.TotalAmount);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithValidData_ShouldUpdateBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1, BookingStatus = "Confirmed" };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Booking { Id = 1, BookingStatus = "Pending" });
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _service.UpdateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Confirmed", result.BookingStatus);
        Assert.NotNull(result.ModifiedDate);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNullBooking_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateBookingAsync(null!));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistentBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { Id = 999, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteBookingAsync(1);

        // Assert
        Assert.True(result);
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteBookingAsync(999);

        // Assert
        Assert.False(result);
        _mockBookingRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, cts.Token))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, cts.Token))
            .ReturnsAsync(true);

        // Act
        await _service.DeleteBookingAsync(1, cts.Token);

        // Assert
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, cts.Token), Times.Once);
    }
}
