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
    public void Constructor_WithNullBookingRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockTourRepository.Object, _mockUserRepository.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, null!, _mockUserRepository.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, _mockTourRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockBookingRepository.Object, _mockTourRepository.Object, _mockUserRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 2, UserId = 1 }
        };
        _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithValidUserId_ReturnsUserBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 2, UserId = 1 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithValidTourId_ReturnsTourBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserId = 1 },
            new Booking { Id = 2, TourId = 1, UserId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidBooking_CreatesBooking()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Price = 1000m };
        var user = new User { Id = 1, Username = "john_doe" };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2000m, result.TotalAmount);
        Assert.True(result.IsActive);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNullBooking_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.CreateBookingAsync(null!));
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistentTour_ThrowsInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { TourId = 999, UserId = 1 };
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Price = 1000m };
        var booking = new Booking { TourId = 1, UserId = 999 };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithValidBooking_UpdatesBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.UpdateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ModifiedDate);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNullBooking_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.UpdateBookingAsync(null!));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistentBooking_ThrowsInvalidOperationException()
    {
        // Arrange
        var booking = new Booking { Id = 999, TourId = 1, UserId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingBooking_ReturnsTrue()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _bookingService.DeleteBookingAsync(1);

        // Assert
        Assert.True(result);
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistentBooking_ReturnsFalse()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _bookingService.DeleteBookingAsync(999);

        // Assert
        Assert.False(result);
        _mockBookingRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_CalculatesTotalAmountCorrectly()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Price = 500m };
        var user = new User { Id = 1, Username = "john_doe" };
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 5 };

        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(booking);

        // Assert
        Assert.Equal(2500m, result.TotalAmount);
    }
}
