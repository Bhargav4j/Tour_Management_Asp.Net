using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Domain.Interfaces.Services.Tests;

public class IBookingServiceTests
{
    private class TestBookingService : IBookingService
    {
        private readonly List<Booking> _bookings = new();

        public Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Booking>>(_bookings.Where(b => b.IsActive).ToList());
        }

        public Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var booking = _bookings.Find(b => b.Id == id && b.IsActive);
            return Task.FromResult(booking);
        }

        public Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var bookings = _bookings.Where(b => b.UserId == userId && b.IsActive).ToList();
            return Task.FromResult<IEnumerable<Booking>>(bookings);
        }

        public Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
        {
            var bookings = _bookings.Where(b => b.TourId == tourId && b.IsActive).ToList();
            return Task.FromResult<IEnumerable<Booking>>(bookings);
        }

        public Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            booking.Id = _bookings.Count + 1;
            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            _bookings.Add(booking);
            return Task.FromResult(booking);
        }

        public Task<Booking> UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            var existing = _bookings.Find(b => b.Id == booking.Id);
            if (existing != null)
            {
                _bookings.Remove(existing);
                booking.ModifiedDate = DateTime.UtcNow;
                _bookings.Add(booking);
            }
            return Task.FromResult(booking);
        }

        public Task<bool> DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
        {
            var booking = _bookings.Find(b => b.Id == id);
            if (booking != null)
            {
                booking.IsActive = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    [Fact]
    public async Task GetAllBookingsAsync_ShouldReturnActiveBookings()
    {
        // Arrange
        var service = new TestBookingService();
        await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 });
        await service.CreateBookingAsync(new Booking { TourId = 2, UserId = 2, NumberOfPeople = 3 });

        // Act
        var result = await service.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllBookingsAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetAllBookingsAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var service = new TestBookingService();
        var createdBooking = await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 });

        // Act
        var result = await service.GetBookingByIdAsync(createdBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdBooking.Id, result.Id);
        Assert.Equal(2, result.NumberOfPeople);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var service = new TestBookingService();

        // Act
        var result = await service.GetBookingByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetBookingByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var service = new TestBookingService();
        await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 100, NumberOfPeople = 2 });
        await service.CreateBookingAsync(new Booking { TourId = 2, UserId = 100, NumberOfPeople = 3 });
        await service.CreateBookingAsync(new Booking { TourId = 3, UserId = 200, NumberOfPeople = 1 });

        // Act
        var result = await service.GetBookingsByUserIdAsync(100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(100, b.UserId));
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithInvalidUserId_ShouldReturnEmpty()
    {
        // Arrange
        var service = new TestBookingService();
        await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 100 });

        // Act
        var result = await service.GetBookingsByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetBookingsByUserIdAsync(1, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var service = new TestBookingService();
        await service.CreateBookingAsync(new Booking { TourId = 50, UserId = 1, NumberOfPeople = 2 });
        await service.CreateBookingAsync(new Booking { TourId = 50, UserId = 2, NumberOfPeople = 4 });
        await service.CreateBookingAsync(new Booking { TourId = 60, UserId = 3, NumberOfPeople = 1 });

        // Act
        var result = await service.GetBookingsByTourIdAsync(50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(50, b.TourId));
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithInvalidTourId_ShouldReturnEmpty()
    {
        // Arrange
        var service = new TestBookingService();
        await service.CreateBookingAsync(new Booking { TourId = 50, UserId = 1 });

        // Act
        var result = await service.GetBookingsByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBookingsByTourIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetBookingsByTourIdAsync(1, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateNewBooking()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = new Booking
        {
            TourId = 1,
            UserId = 1,
            NumberOfPeople = 3,
            TotalAmount = 1500.00m,
            BookingStatus = "Pending"
        };

        // Act
        var result = await service.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(3, result.NumberOfPeople);
        Assert.Equal(1500.00m, result.TotalAmount);
        Assert.True(result.IsActive);
        Assert.True(result.BookingDate <= DateTime.UtcNow);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateBookingAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = new Booking { TourId = 1, UserId = 1 };
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.CreateBookingAsync(booking, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldUpdateExistingBooking()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = await service.CreateBookingAsync(new Booking
        {
            TourId = 1,
            UserId = 1,
            BookingStatus = "Pending"
        });
        booking.BookingStatus = "Confirmed";

        // Act
        var result = await service.UpdateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Confirmed", result.BookingStatus);
        Assert.NotNull(result.ModifiedDate);
    }

    [Fact]
    public async Task UpdateBookingAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 1 });
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.UpdateBookingAsync(booking, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 1 });

        // Act
        var result = await service.DeleteBookingAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var service = new TestBookingService();

        // Act
        var result = await service.DeleteBookingAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldSoftDelete()
    {
        // Arrange
        var service = new TestBookingService();
        var booking = await service.CreateBookingAsync(new Booking { TourId = 1, UserId = 1 });

        // Act
        await service.DeleteBookingAsync(booking.Id);
        var deletedBooking = await service.GetBookingByIdAsync(booking.Id);

        // Assert
        Assert.Null(deletedBooking);
    }

    [Fact]
    public async Task DeleteBookingAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestBookingService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.DeleteBookingAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }
}
