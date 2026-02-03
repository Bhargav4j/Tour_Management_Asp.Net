using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, Email = "user1@example.com" },
            new Booking { Id = 2, TourId = 2, Email = "user2@example.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourId = 1, Email = "test@example.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(booking);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithValidEmail_ShouldReturnBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, Email = "test@example.com" },
            new Booking { Id = 2, TourId = 2, Email = "test@example.com" }
        };
        _mockRepository.Setup(r => r.GetByUserEmailAsync("test@example.com", default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetByUserEmailAsync("test@example.com", default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidBooking_ShouldReturnCreatedBooking()
    {
        // Arrange
        var booking = new Booking { TourId = 1, Email = "test@example.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default))
            .ReturnsAsync((Booking b, CancellationToken ct) =>
            {
                b.Id = 1;
                return b;
            });

        // Act
        var result = await _service.CreateAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(result.IsActive);
        Assert.Equal("test@example.com", result.CreatedBy);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_ShouldUpdateBooking()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, TourId = 1, Email = "old@example.com" };
        var updatedBooking = new Booking { TourId = 2, Email = "new@example.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingBooking);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updatedBooking);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var updatedBooking = new Booking { TourId = 1, Email = "test@example.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, updatedBooking));
        _mockRepository.Verify(r => r.GetByIdAsync(999, default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), default), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteBooking()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, default)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.ExistsAsync(1, default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
        _mockRepository.Verify(r => r.ExistsAsync(999, default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), default), Times.Never);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithEmptyEmail_ShouldReturnEmptyList()
    {
        // Arrange
        var bookings = new List<Booking>();
        _mockRepository.Setup(r => r.GetByUserEmailAsync(string.Empty, default)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByUserEmailAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetByUserEmailAsync(string.Empty, default), Times.Once);
    }
}
