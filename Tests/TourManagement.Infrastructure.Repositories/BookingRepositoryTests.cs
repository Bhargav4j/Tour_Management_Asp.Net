using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.AddRange(
            new Booking { TourName = "Tour1", Email = "user1@test.com", IsActive = true },
            new Booking { TourName = "Tour2", Email = "user2@test.com", IsActive = true },
            new Booking { TourName = "Tour3", Email = "user3@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActiveBookings_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.Add(new Booking { TourName = "Tour1", Email = "user@test.com", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Paris Tour", Email = "test@test.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(booking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Paris Tour", Email = "test@test.com", IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(booking.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ReturnsBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.AddRange(
            new Booking { UserId = 1, TourName = "Tour1", Email = "user@test.com", IsActive = true },
            new Booking { UserId = 1, TourName = "Tour2", Email = "user@test.com", IsActive = true },
            new Booking { UserId = 2, TourName = "Tour3", Email = "other@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByUserIdAsync_WithInvalidUserId_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.Add(new Booking { UserId = 1, TourName = "Tour1", Email = "user@test.com", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ExcludesInactiveBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.AddRange(
            new Booking { UserId = 1, TourName = "Active", Email = "user@test.com", IsActive = true },
            new Booking { UserId = 1, TourName = "Inactive", Email = "user@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Active", result.First().TourName);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithValidTourId_ReturnsBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.AddRange(
            new Booking { TourId = 1, TourName = "Paris Tour", Email = "user1@test.com", IsActive = true },
            new Booking { TourId = 1, TourName = "Paris Tour", Email = "user2@test.com", IsActive = true },
            new Booking { TourId = 2, TourName = "Rome Tour", Email = "user3@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTourIdAsync_WithInvalidTourId_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.Add(new Booking { TourId = 1, TourName = "Tour1", Email = "user@test.com", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ExcludesInactiveBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        context.Bookings.AddRange(
            new Booking { TourId = 1, TourName = "Active", Email = "user1@test.com", IsActive = true },
            new Booking { TourId = 1, TourName = "Inactive", Email = "user2@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Active", result.First().TourName);
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsBooking()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "New Tour", Email = "test@test.com", IsActive = true };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task AddAsync_SavesBookingToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "New Tour", Email = "test@test.com", IsActive = true };

        // Act
        await repository.AddAsync(booking);

        // Assert
        var savedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(savedBooking);
        Assert.Equal("New Tour", savedBooking.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBooking()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Old Name", Email = "test@test.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        context.Entry(booking).State = EntityState.Detached;

        // Act
        booking.TourName = "New Name";
        await repository.UpdateAsync(booking);

        // Assert
        var updatedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(updatedBooking);
        Assert.Equal("New Name", updatedBooking.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Tour to Delete", Email = "test@test.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(booking.Id);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(deletedBooking);
        Assert.False(deletedBooking.IsActive);
        Assert.NotNull(deletedBooking.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveBooking_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Paris Tour", Email = "test@test.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourName = "Paris Tour", Email = "test@test.com", IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }
}
