using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Repositories;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveBookings()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserEmail = "user1@test.com", IsActive = true },
            new Booking { Id = 2, TourId = 2, UserEmail = "user2@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("user1@test.com", result.First().UserEmail);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.Add(new Booking { Id = 1, TourId = 10, UserEmail = "user@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(10, result.TourId);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsUserBookings()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserEmail = "user@test.com", IsActive = true },
            new Booking { Id = 2, TourId = 2, UserEmail = "user@test.com", IsActive = true },
            new Booking { Id = 3, TourId = 3, UserEmail = "other@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetByUserEmailAsync("user@test.com");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsNewBooking()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourId = 5, UserEmail = "newbooking@test.com", IsActive = true };

        var result = await repository.AddAsync(booking);

        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingBooking()
    {
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { TourId = 1, UserEmail = "user@test.com", FirstName = "Original", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        booking.FirstName = "Updated";
        var repository = new BookingRepository(context, _mockLogger.Object);
        await repository.UpdateAsync(booking);

        var updated = await context.Bookings.FindAsync(booking.Id);
        Assert.Equal("Updated", updated!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBooking()
    {
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourId = 1, UserEmail = "user@test.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        await repository.DeleteAsync(1);

        var deleted = await context.Bookings.FindAsync(1);
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserEmail = "exists@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }
}
