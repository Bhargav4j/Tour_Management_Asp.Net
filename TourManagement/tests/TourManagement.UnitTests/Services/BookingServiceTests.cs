using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for BookingService
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockTourRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _bookingService = new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void BookingService_Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var service = new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
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
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetAllBookingsAsync());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingByIdAsync(1));
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithValidUserId_ShouldReturnBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 2, UserId = 1 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingsByUserIdAsync(1));
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithValidTourId_ShouldReturnBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 1, UserId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetBookingsByTourIdAsync(1));
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidBooking_ShouldCreateAndReturnBooking()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Price = 100m };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 };
        var createdBooking = new Booking { Id = 1, TourId = 1, UserId = 1, NumberOfPeople = 2 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(booking.IsActive);
        Assert.NotEqual(default(DateTime), booking.CreatedDate);
        Assert.Equal(200m, booking.TotalAmount);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistentTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 999, UserId = 1, NumberOfPeople = 2 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(booking));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCalculateTotalAmount()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Price = 150.50m };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 3 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.Equal(451.50m, booking.TotalAmount);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Price = 100m };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 };
        var beforeCreate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(booking.CreatedDate >= beforeCreate && booking.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Price = 100m };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2, IsActive = false };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithExistingBooking_ShouldUpdateBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        var existingBooking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(booking);

        // Assert
        Assert.NotEqual(default(DateTime), booking.ModifiedDate);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { Id = 999, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.UpdateBookingAsync(booking));
        Assert.Contains("Booking with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldSetModifiedDateToUtcNow()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        var existingBooking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        var beforeUpdate = DateTime.UtcNow;
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(booking);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(booking.ModifiedDate);
        Assert.True(booking.ModifiedDate >= beforeUpdate && booking.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task UpdateBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingBooking_ShouldDeleteBooking()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteBookingAsync(1);

        // Assert
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.DeleteBookingAsync(999));
        Assert.Contains("Booking with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.DeleteBookingAsync(1));
    }
}
