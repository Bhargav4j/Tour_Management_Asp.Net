using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

/// <summary>
/// Unit tests for BookingRepository
/// </summary>
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
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        context.Bookings.AddRange(
            new Booking { Id = 1, TourName = "Tour 1", IsActive = true },
            new Booking { Id = 2, TourName = "Tour 2", IsActive = true },
            new Booking { Id = 3, TourName = "Tour 3", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        var booking = new Booking { Id = 1, TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Booking", result.TourName);
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

        var booking = new Booking { Id = 1, TourName = "Test Booking", IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsBookingsForUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        context.Bookings.AddRange(
            new Booking { Email = "user@test.com", TourName = "Tour 1", IsActive = true },
            new Booking { Email = "user@test.com", TourName = "Tour 2", IsActive = true },
            new Booking { Email = "other@test.com", TourName = "Tour 3", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserEmailAsync("user@test.com");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal("user@test.com", b.Email));
    }

    [Fact]
    public async Task AddAsync_AddsBookingSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        var booking = new Booking
        {
            TourName = "New Booking",
            Email = "test@test.com",
            FirstName = "John",
            IsActive = true
        };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Booking", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBookingSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        var booking = new Booking { TourName = "Old Booking", Status = "Pending", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        booking.Status = "Confirmed";
        await repository.UpdateAsync(booking);

        // Assert
        var updated = await context.Bookings.FindAsync(booking.Id);
        Assert.Equal("Confirmed", updated!.Status);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBooking()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        var booking = new Booking { TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(booking.Id);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.False(deletedBooking!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        var booking = new Booking { TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingBookings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        context.Bookings.AddRange(
            new Booking { TourName = "Paris Tour", FirstName = "John", IsActive = true },
            new Booking { TourName = "London Tour", FirstName = "Jane", IsActive = true },
            new Booking { TourName = "Rome Tour", FirstName = "Bob", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Empty(result);
    }
}
