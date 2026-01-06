using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private class MockBookingRepository : IBookingRepository
    {
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public bool ThrowException { get; set; }

        public Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Bookings.AsEnumerable());
        }

        public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Bookings.FirstOrDefault(b => b.Id == id));
        }

        public Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Bookings.Where(b => b.UserId == userId).AsEnumerable());
        }

        public Task<Booking> AddAsync(Booking entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            entity.Id = Bookings.Count + 1;
            Bookings.Add(entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var existing = Bookings.FirstOrDefault(b => b.Id == entity.Id);
            if (existing != null)
            {
                var index = Bookings.IndexOf(existing);
                Bookings[index] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var booking = Bookings.FirstOrDefault(b => b.Id == id);
            if (booking != null)
            {
                Bookings.Remove(booking);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Bookings.Any(b => b.Id == id));
        }

        public Task<IEnumerable<Booking>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Bookings.Where(b => b.TourId == tourId).AsEnumerable());
        }
    }

    private class MockLogger : ILogger<BookingService>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var repository = new MockBookingRepository();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(repository, null!));
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsAllBookings()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1 });
        repository.Bookings.Add(new Booking { Id = 2, UserId = 2, TourId = 2 });
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllBookingsAsync_ReturnsEmptyList_WhenNoBookings()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetAllBookingsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllBookingsAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockBookingRepository { ThrowException = true };
        var service = new BookingService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetAllBookingsAsync());
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsBooking_WhenExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1 });
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetBookingByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetBookingByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockBookingRepository { ThrowException = true };
        var service = new BookingService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetBookingByIdAsync(1));
    }

    [Fact]
    public async Task GetUserBookingsAsync_ReturnsUserBookings()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1 });
        repository.Bookings.Add(new Booking { Id = 2, UserId = 1, TourId = 2 });
        repository.Bookings.Add(new Booking { Id = 3, UserId = 2, TourId = 1 });
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetUserBookingsAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
    }

    [Fact]
    public async Task GetUserBookingsAsync_ReturnsEmpty_WhenNoBookings()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());

        // Act
        var result = await service.GetUserBookingsAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserBookingsAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockBookingRepository { ThrowException = true };
        var service = new BookingService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetUserBookingsAsync(1));
    }

    [Fact]
    public async Task CreateBookingAsync_AddsBooking()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };

        // Act
        var result = await service.CreateBookingAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.True(result.IsActive);
        Assert.Equal("Confirmed", result.Status);
        Assert.Single(repository.Bookings);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsBookingDate()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1 };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await service.CreateBookingAsync(booking);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(result.BookingDate, beforeCreation, afterCreation);
        Assert.InRange(result.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public async Task CreateBookingAsync_SetsStatusToConfirmed()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1 };

        // Act
        var result = await service.CreateBookingAsync(booking);

        // Assert
        Assert.Equal("Confirmed", result.Status);
    }

    [Fact]
    public async Task CreateBookingAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockBookingRepository { ThrowException = true };
        var service = new BookingService(repository, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.CreateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_UpdatesBooking_WhenExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 2 });
        var service = new BookingService(repository, new MockLogger());
        var updatedBooking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 3 };

        // Act
        await service.UpdateBookingAsync(updatedBooking);

        // Assert
        var booking = repository.Bookings.First();
        Assert.Equal(3, booking.NumberOfPeople);
        Assert.NotNull(booking.ModifiedDate);
    }

    [Fact]
    public async Task UpdateBookingAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());
        var booking = new Booking { Id = 999, UserId = 1, TourId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateBookingAsync(booking));
    }

    [Fact]
    public async Task UpdateBookingAsync_SetsModifiedDate()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1 });
        var service = new BookingService(repository, new MockLogger());
        var updatedBooking = new Booking { Id = 1, UserId = 1, TourId = 1 };
        var beforeUpdate = DateTime.UtcNow;

        // Act
        await service.UpdateBookingAsync(updatedBooking);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(updatedBooking.ModifiedDate);
        Assert.InRange(updatedBooking.ModifiedDate.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task DeleteBookingAsync_DeletesBooking_WhenExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        repository.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1 });
        var service = new BookingService(repository, new MockLogger());

        // Act
        await service.DeleteBookingAsync(1);

        // Assert
        Assert.Empty(repository.Bookings);
    }

    [Fact]
    public async Task DeleteBookingAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteBookingAsync(999));
    }

    [Fact]
    public async Task DeleteBookingAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockBookingRepository { ThrowException = true };
        var service = new BookingService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.DeleteBookingAsync(1));
    }

    [Fact]
    public async Task GetAllBookingsAsync_SupportsCancellation()
    {
        // Arrange
        var repository = new MockBookingRepository();
        var service = new BookingService(repository, new MockLogger());
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetAllBookingsAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
