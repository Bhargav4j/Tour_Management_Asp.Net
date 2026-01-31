using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Domain.Interfaces.Repositories.Tests;

public class IBookingRepositoryTests
{
    private class TestBookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new();

        public Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Booking>>(_bookings);
        }

        public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var booking = _bookings.Find(b => b.Id == id);
            return Task.FromResult(booking);
        }

        public Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var bookings = _bookings.Where(b => b.UserId == userId).ToList();
            return Task.FromResult<IEnumerable<Booking>>(bookings);
        }

        public Task<IEnumerable<Booking>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
        {
            var bookings = _bookings.Where(b => b.TourId == tourId).ToList();
            return Task.FromResult<IEnumerable<Booking>>(bookings);
        }

        public Task<Booking> AddAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            booking.Id = _bookings.Count + 1;
            _bookings.Add(booking);
            return Task.FromResult(booking);
        }

        public Task<Booking> UpdateAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            var existing = _bookings.Find(b => b.Id == booking.Id);
            if (existing != null)
            {
                _bookings.Remove(existing);
                _bookings.Add(booking);
            }
            return Task.FromResult(booking);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var booking = _bookings.Find(b => b.Id == id);
            if (booking != null)
            {
                _bookings.Remove(booking);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_bookings.Exists(b => b.Id == id));
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var repository = new TestBookingRepository();
        await repository.AddAsync(new Booking { TourId = 1, UserId = 1 });
        await repository.AddAsync(new Booking { TourId = 2, UserId = 2 });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var addedBooking = await repository.AddAsync(new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2 });

        // Act
        var result = await repository.GetByIdAsync(addedBooking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedBooking.Id, result.Id);
        Assert.Equal(2, result.NumberOfPeople);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var repository = new TestBookingRepository();

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var repository = new TestBookingRepository();
        await repository.AddAsync(new Booking { TourId = 1, UserId = 100 });
        await repository.AddAsync(new Booking { TourId = 2, UserId = 100 });
        await repository.AddAsync(new Booking { TourId = 3, UserId = 200 });

        // Act
        var result = await repository.GetByUserIdAsync(100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(100, b.UserId));
    }

    [Fact]
    public async Task GetByUserIdAsync_WithInvalidUserId_ShouldReturnEmpty()
    {
        // Arrange
        var repository = new TestBookingRepository();
        await repository.AddAsync(new Booking { TourId = 1, UserId = 100 });

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByUserIdAsync(1, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var repository = new TestBookingRepository();
        await repository.AddAsync(new Booking { TourId = 50, UserId = 1 });
        await repository.AddAsync(new Booking { TourId = 50, UserId = 2 });
        await repository.AddAsync(new Booking { TourId = 60, UserId = 3 });

        // Act
        var result = await repository.GetByTourIdAsync(50);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(50, b.TourId));
    }

    [Fact]
    public async Task GetByTourIdAsync_WithInvalidTourId_ShouldReturnEmpty()
    {
        // Arrange
        var repository = new TestBookingRepository();
        await repository.AddAsync(new Booking { TourId = 50, UserId = 1 });

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByTourIdAsync(1, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddBooking()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 3, TotalAmount = 1500.00m };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(3, result.NumberOfPeople);
        Assert.Equal(1500.00m, result.TotalAmount);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = new Booking { TourId = 1, UserId = 1 };
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.AddAsync(booking, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingBooking()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = await repository.AddAsync(new Booking { TourId = 1, UserId = 1, BookingStatus = "Pending" });
        booking.BookingStatus = "Confirmed";

        // Act
        var result = await repository.UpdateAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Confirmed", result.BookingStatus);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = await repository.AddAsync(new Booking { TourId = 1, UserId = 1 });
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.UpdateAsync(booking, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = await repository.AddAsync(new Booking { TourId = 1, UserId = 1 });

        // Act
        var result = await repository.DeleteAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestBookingRepository();

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.DeleteAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var booking = await repository.AddAsync(new Booking { TourId = 1, UserId = 1 });

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestBookingRepository();

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestBookingRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.ExistsAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }
}
