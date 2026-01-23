using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests.Services;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTourRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _bookingService = new BookingService(
            _mockBookingRepository.Object,
            _mockUserRepository.Object,
            _mockTourRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullBookingRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockUserRepository.Object, _mockTourRepository.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, null!, _mockTourRepository.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, _mockUserRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, _mockUserRepository.Object, _mockTourRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1 },
            new Booking { Id = 2, UserId = 2, TourId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1 },
            new Booking { Id = 2, UserId = 1, TourId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithInvalidUserId_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1 },
            new Booking { Id = 2, UserId = 2, TourId = 1 }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithInvalidTourId_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidBooking_ShouldCreateAndReturnBooking()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            TotalAmount = 1000m
        };
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.Equal("System", result.CreatedBy);
        Assert.Equal("Confirmed", result.Status);
        Assert.NotEqual(default(DateTime), result.CreatedDate);
        Assert.NotEqual(default(DateTime), result.BookingDate);
        _mockUserRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockTourRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidUserId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { UserId = 999, TourId = 1 };
        _mockUserRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _bookingService.CreateAsync(booking));
        Assert.Contains("User", exception.Message);
        Assert.Contains("not found", exception.Message);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTourId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { UserId = 1, TourId = 999 };
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockTourRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _bookingService.CreateAsync(booking));
        Assert.Contains("Tour", exception.Message);
        Assert.Contains("not found", exception.Message);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingBooking_ShouldUpdateBooking()
    {
        // Arrange
        var existingBooking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            TotalAmount = 1000m,
            Status = "Pending"
        };
        var updatedBooking = new Booking
        {
            NumberOfPeople = 3,
            TotalAmount = 1500m,
            Status = "Confirmed"
        };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateAsync(1, updatedBooking);

        // Assert
        Assert.Equal(3, existingBooking.NumberOfPeople);
        Assert.Equal(1500m, existingBooking.TotalAmount);
        Assert.Equal("Confirmed", existingBooking.Status);
        Assert.NotNull(existingBooking.ModifiedDate);
        Assert.Equal("System", existingBooking.ModifiedBy);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { NumberOfPeople = 3 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _bookingService.UpdateAsync(999, booking));
        Assert.Contains("not found", exception.Message);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingBooking_ShouldDeleteBooking()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteAsync(1);

        // Assert
        _mockBookingRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _bookingService.DeleteAsync(999));
        Assert.Contains("not found", exception.Message);
        _mockBookingRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
