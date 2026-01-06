using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
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
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, Email = "test1@test.com" },
            new Booking { Id = 2, Email = "test2@test.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetAllBookingsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsBooking_WhenExists()
    {
        var booking = new Booking { Id = 1, Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _service.GetBookingByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsNull_WhenNotExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var result = await _service.GetBookingByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateBookingAsync_ThrowsArgumentNullException_WhenNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateBookingAsync(null!));
    }

    [Fact]
    public async Task CreateBookingAsync_SetsDates_AndReturnsCreated()
    {
        var booking = new Booking { Email = "new@test.com", FirstName = "Test" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _service.CreateBookingAsync(booking);

        Assert.NotNull(result);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public async Task UpdateBookingAsync_ThrowsArgumentNullException_WhenNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateBookingAsync(1, null!));
    }

    [Fact]
    public async Task DeleteBookingAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteBookingAsync(999));
    }

    [Fact]
    public async Task GetBookingsByUserEmailAsync_ReturnsMatchingBookings()
    {
        var bookings = new List<Booking> { new Booking { Email = "user@test.com" } };
        _mockRepository.Setup(r => r.GetByUserEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetBookingsByUserEmailAsync("user@test.com");

        Assert.Single(result);
    }
}
