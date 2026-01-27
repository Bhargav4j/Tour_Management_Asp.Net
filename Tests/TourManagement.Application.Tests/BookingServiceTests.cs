using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests;

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
    public async Task GetAllBookingsAsync_WhenCalled_ShouldReturnAllBookings()
    {
        // Arrange
        var expectedBookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1 },
            new Booking { Id = 2, UserId = 2, TourId = 2 }
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
    public async Task GetAllBookingsAsync_WhenNoBookings_ShouldReturnEmptyList()
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
        var expectedBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBooking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task GetUserBookingsAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var userId = 1;
        var expectedBookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = userId, TourId = 1 },
            new Booking { Id = 2, UserId = userId, TourId = 2 }
        };
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBookings);

        // Act
        var result = await _bookingService.GetUserBookingsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(userId, b.UserId));
        _mockBookingRepository.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserBookingsAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = 999;
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetUserBookingsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookingsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetUserBookingsAsync(1));
    }

    [Fact]
    public async Task GetTourBookingsAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var tourId = 1;
        var expectedBookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = tourId },
            new Booking { Id = 2, UserId = 2, TourId = tourId }
        };
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBookings);

        // Act
        var result = await _bookingService.GetTourBookingsAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(tourId, b.TourId));
        _mockBookingRepository.Verify(r => r.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTourBookingsAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var tourId = 999;
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _bookingService.GetTourBookingsAsync(tourId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTourBookingsAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByTourIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.GetTourBookingsAsync(1));
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ShouldReturnCreatedBooking()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Price = 1000m };
        var newBooking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };
        var createdBooking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 2 };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _bookingService.CreateBookingAsync(newBooking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(newBooking.IsActive);
        Assert.NotEqual(default(DateTime), newBooking.CreatedDate);
        Assert.Equal(2000m, newBooking.TotalAmount);
        _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var newBooking = new Booking { UserId = 999, TourId = 1, NumberOfPeople = 2 };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.CreateBookingAsync(newBooking));
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistingTour_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        var newBooking = new Booking { UserId = 1, TourId = 999, NumberOfPeople = 2 };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockTourRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.CreateBookingAsync(newBooking));
        Assert.Contains("Tour with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCalculateTotalAmount()
    {
        // Arrange
        var user = new User { Id = 1 };
        var tour = new Tour { Id = 1, Price = 500m };
        var newBooking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 3 };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newBooking);

        // Act
        await _bookingService.CreateBookingAsync(newBooking);

        // Assert
        Assert.Equal(1500m, newBooking.TotalAmount);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var user = new User { Id = 1 };
        var tour = new Tour { Id = 1, Price = 1000m };
        var newBooking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockTourRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);
        _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.CreateBookingAsync(newBooking));
    }

    [Fact]
    public async Task UpdateBookingAsync_WithExistingBooking_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        var updatedBooking = new Booking { Id = 1, UserId = 1, TourId = 2 };

        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(updatedBooking);

        // Assert
        Assert.NotEqual(default(DateTime), updatedBooking.ModifiedDate);
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockBookingRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var updatedBooking = new Booking { Id = 999, UserId = 1, TourId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.UpdateBookingAsync(updatedBooking));
        Assert.Contains("Booking with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        var updatedBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };

        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.UpdateBookingAsync(updatedBooking);

        // Assert
        Assert.NotNull(updatedBooking.ModifiedDate);
        Assert.NotEqual(default(DateTime), updatedBooking.ModifiedDate);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingBooking_ShouldDeleteSuccessfully()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bookingService.DeleteBookingAsync(1);

        // Assert
        _mockBookingRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockBookingRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistingBooking_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockBookingRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.DeleteBookingAsync(999));
        Assert.Contains("Booking with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        _mockBookingRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockBookingRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _bookingService.DeleteBookingAsync(1));
    }

    [Fact]
    public async Task GetAllBookingsAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _mockBookingRepository.Setup(r => r.GetAllAsync(cancellationToken))
            .ReturnsAsync(new List<Booking>());

        // Act
        await _bookingService.GetAllBookingsAsync(cancellationToken);

        // Assert
        _mockBookingRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }
}
