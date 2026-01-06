using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private class MockLogger : ILogger<BookingRepository>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 2, UserId = 1, TourId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookingIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user1 = new User { Id = 1, Email = "user1@test.com", IsActive = true };
        var user2 = new User { Id = 2, Email = "user2@test.com", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Users.Add(user1);
        context.Users.Add(user2);
        context.Tours.Add(tour);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 2, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 3, UserId = 2, TourId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsEmpty_WhenNoBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsOnlyActiveBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 2, UserId = 1, TourId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsTourBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        var tour1 = new Tour { Id = 1, TourName = "Tour 1", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Tour 2", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour1);
        context.Tours.Add(tour2);
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 2, UserId = 1, TourId = 1, IsActive = true });
        context.Bookings.Add(new Booking { Id = 3, UserId = 1, TourId = 2, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsEmpty_WhenNoBookings()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_AddsBookingToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Single(context.Bookings);
    }

    [Fact]
    public async Task AddAsync_ReturnsBookingWithGeneratedId()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());
        var booking = new Booking { UserId = 1, TourId = 1 };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBooking()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        context.Entry(booking).State = EntityState.Detached;

        var repository = new BookingRepository(context, new MockLogger());
        var updatedBooking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 3, IsActive = true };

        // Act
        await repository.UpdateAsync(updatedBooking);

        // Assert
        var result = await context.Bookings.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal(3, result.NumberOfPeople);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var booking = await context.Bookings.FindAsync(1);
        Assert.NotNull(booking);
        Assert.False(booking.IsActive);
        Assert.NotNull(booking.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_SetsModifiedDate()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());
        var beforeDelete = DateTime.UtcNow;

        // Act
        await repository.DeleteAsync(1);
        var afterDelete = DateTime.UtcNow;

        // Assert
        var booking = await context.Bookings.FindAsync(1);
        Assert.NotNull(booking);
        Assert.NotNull(booking.ModifiedDate);
        Assert.InRange(booking.ModifiedDate.Value, beforeDelete, afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenBookingNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act & Assert (should not throw)
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenBookingExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenBookingIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Bookings.Add(new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenBookingNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_SupportsCancellation()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, new MockLogger());
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
