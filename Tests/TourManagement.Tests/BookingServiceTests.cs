using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<ITourRepository> _mockTourRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockTourRepository = new Mock<ITourRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _bookingService = new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullBookingRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            null!,
            _mockTourRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            _mockBookingRepository.Object,
            null!,
            _mockUserRepository.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(
            _mockBookingRepository.Object,
            _mockTourRepository.Object,
            null!,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
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
        var expectedBookings = new List<Booking>
        {
            new Booking { BookingId = 1, Email = "user1@test.com" },
            new Booking { BookingId = 2, Email = "user2@test.com" }
        };
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBookings);

        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllBookingsAsync_WithEmptyResult_ShouldReturnEmptyCollection()
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
        var bookingId = 1;
        var expectedBooking = new Booking { BookingId = bookingId, Email = "test@example.com" };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBooking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookingId, result.BookingId);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var bookingId = 999;
        _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

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
    public async Task GetUserBookingsAsync_WithValidEmail_ShouldReturnUserBookings()
    {
        // Arrange
        var email = "test@example.com";
        var expectedBookings = new List<Booking>
        {
            new Booking { BookingId = 1, Email = email },
            new Booking { BookingId = 2, Email = email }
        };
        _mockBookingRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBookings);

        // Act
        var result = await _bookingService.GetUserBookingsAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(email, b.Email));
    }

    [Fact]
    public async Task GetUserBookingsAsync_WithNoBookings_ShouldReturnEmptyCollection()
    {
        // Arrange
        var email = "test@example.com";
        _mockBookingRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetUserBookingsAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookingsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByUserEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetUserBookingsAsync("test@example.com"));
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ShouldCreateAndReturnBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(booking.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(booking.IsActive);
        Assert.Equal("Confirmed", booking.Status);
        Assert.NotEqual(default(DateTime), booking.BookingDate);
        Assert.NotEqual(default(DateTime), booking.CreatedDate);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldSetBookingDateToUtcNow()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        var beforeCreate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(booking.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(booking.BookingDate >= beforeCreate);
        Assert.True(booking.BookingDate <= afterCreate);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        var beforeCreate = DateTime.UtcNow;
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(booking.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(booking.CreatedDate >= beforeCreate);
        Assert.True(booking.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldSetStatusToConfirmed()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(booking.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidTourId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 999, Email = "test@example.com" };
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "nonexistent@example.com" };
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(booking.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        _mockTourRepository.Setup(r => r.ExistsAsync(booking.TourId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithExistingBooking_ShouldUpdateBooking()
    {
        // Arrange
        var booking = new Booking { BookingId = 1, TourId = 1, Email = "test@example.com" };
        var existingBooking = new Booking { BookingId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(booking);

        // Assert
        Assert.NotEqual(default(DateTime), booking.ModifiedDate);
        _mockBookingRepository.Verify(r => r.UpdateAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldSetModifiedDateToUtcNow()
    {
        // Arrange
        var booking = new Booking { BookingId = 1, TourId = 1, Email = "test@example.com" };
        var existingBooking = new Booking { BookingId = 1 };
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
        Assert.True(booking.ModifiedDate >= beforeUpdate);
        Assert.True(booking.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { BookingId = 999 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var booking = new Booking { BookingId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingBookingId_ShouldDeleteBooking()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository.Setup(r => r.ExistsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(bookingId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteBookingAsync(bookingId);

        // Assert
        _mockBookingRepository.Verify(r => r.DeleteAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistingBookingId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var bookingId = 999;
        _mockBookingRepository.Setup(r => r.ExistsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.DeleteBookingAsync(bookingId));
    }

    [Fact]
    public async Task DeleteBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var bookingId = 1;
        _mockBookingRepository.Setup(r => r.ExistsAsync(bookingId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.DeleteBookingAsync(bookingId));
    }
}
