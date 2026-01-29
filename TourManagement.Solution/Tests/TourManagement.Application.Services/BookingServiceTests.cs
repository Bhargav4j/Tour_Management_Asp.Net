using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Tests.Application.Services;

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
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1, UserEmail = "user1@test.com" },
            new Booking { Id = 2, TourId = 2, UserEmail = "user2@test.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetAllBookingsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllBookingsAsync_WhenRepositoryThrows_ThrowsTourManagementException()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<TourManagementException>(() => _service.GetAllBookingsAsync());
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ReturnsBooking()
    {
        var booking = new Booking { Id = 1, TourId = 10, UserEmail = "user@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _service.GetBookingByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetBookingByIdAsync(999));
    }

    [Fact]
    public async Task GetUserBookingsAsync_WithValidEmail_ReturnsBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserEmail = "user@test.com" }
        };
        _mockRepository.Setup(r => r.GetByUserEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetUserBookingsAsync("user@test.com");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserBookingsAsync_WhenRepositoryThrows_ThrowsTourManagementException()
    {
        _mockRepository.Setup(r => r.GetByUserEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<TourManagementException>(() => _service.GetUserBookingsAsync("user@test.com"));
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidBooking_ReturnsBooking()
    {
        var booking = new Booking { TourId = 1, UserEmail = "user@test.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var result = await _service.CreateBookingAsync(booking);

        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(DateTime.MinValue, result.BookingDate);
        Assert.NotEqual(DateTime.MinValue, result.CreatedDate);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsBookingDateCreatedDateAndIsActive()
    {
        var booking = new Booking { TourId = 1, UserEmail = "user@test.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        await _service.CreateBookingAsync(booking);

        Assert.True(booking.IsActive);
        Assert.NotEqual(DateTime.MinValue, booking.BookingDate);
        Assert.NotEqual(DateTime.MinValue, booking.CreatedDate);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithExistingBooking_UpdatesBooking()
    {
        var existingBooking = new Booking { Id = 1, TourId = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var updatedBooking = new Booking { Id = 1, TourId = 1, FirstName = "Updated" };
        await _service.UpdateBookingAsync(updatedBooking);

        Assert.NotNull(updatedBooking.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithNonExistingBooking_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var booking = new Booking { Id = 999 };
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task DeleteBookingAsync_WithExistingId_DeletesBooking()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.DeleteBookingAsync(1);

        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithNonExistingId_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteBookingAsync(999));
    }
}
