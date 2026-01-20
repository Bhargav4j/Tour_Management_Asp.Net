using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests;

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
    public void BookingService_Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockLogger.Object));
    }

    [Fact]
    public void BookingService_Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
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
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsEmptyList_WhenNoBookingsExist()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsBooking_WhenBookingExists()
    {
        // Arrange
        var booking = new Booking { Id = 1, UserId = 100, TourId = 200 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(100, result.UserId);
        Assert.Equal(200, result.TourId);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
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
    public async Task GetBookingsByUserIdAsync_ReturnsBookingsForUser()
    {
        // Arrange
        var userId = 100;
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = userId, TourId = 1 },
            new Booking { Id = 2, UserId = userId, TourId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(userId, b.UserId));
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ReturnsEmptyList_WhenNoBookingsForUser()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_ReturnsBookingsForTour()
    {
        // Arrange
        var tourId = 200;
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = tourId },
            new Booking { Id = 2, UserId = 2, TourId = tourId },
            new Booking { Id = 3, UserId = 3, TourId = tourId }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.All(result, b => Assert.Equal(tourId, b.TourId));
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_ReturnsEmptyList_WhenNoBookingsForTour()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateBookingAsync_CreatesAndReturnsBooking()
    {
        // Arrange
        var booking = new Booking { UserId = 100, TourId = 200, NumberOfPeople = 2, TotalAmount = 2000m };
        var createdBooking = new Booking { Id = 1, UserId = 100, TourId = 200, NumberOfPeople = 2, TotalAmount = 2000m };
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(booking.IsActive);
        Assert.Equal("Pending", booking.Status);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsCreatedDate()
    {
        // Arrange
        var booking = new Booking { UserId = 100, TourId = 200 };
        var beforeCreate = DateTime.UtcNow;
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(booking.CreatedDate >= beforeCreate && booking.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsBookingDate()
    {
        // Arrange
        var booking = new Booking { UserId = 100, TourId = 200 };
        var beforeCreate = DateTime.UtcNow;
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(booking.BookingDate >= beforeCreate && booking.BookingDate <= afterCreate);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsIsActiveToTrue()
    {
        // Arrange
        var booking = new Booking { UserId = 100, TourId = 200, IsActive = false };
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsStatusToPending()
    {
        // Arrange
        var booking = new Booking { UserId = 100, TourId = 200, Status = "Confirmed" };
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public async Task UpdateBookingAsync_UpdatesBooking_WhenBookingExists()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 100, TourId = 200 };
        var updatedBooking = new Booking { Id = 1, UserId = 100, TourId = 200, Status = "Confirmed" };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(updatedBooking);

        // Assert
        Assert.NotNull(updatedBooking.ModifiedDate);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_ThrowsEntityNotFoundException_WhenBookingDoesNotExist()
    {
        // Arrange
        var booking = new Booking { Id = 999, UserId = 100, TourId = 200 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_SetsModifiedDate()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 100, TourId = 200 };
        var booking = new Booking { Id = 1, UserId = 100, TourId = 200 };
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
    public async Task DeleteBookingAsync_DeletesBooking_WhenBookingExists()
    {
        // Arrange
        var booking = new Booking { Id = 1, UserId = 100, TourId = 200 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteBookingAsync(1);

        // Assert
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_ThrowsEntityNotFoundException_WhenBookingDoesNotExist()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.DeleteBookingAsync(999));
    }
}
