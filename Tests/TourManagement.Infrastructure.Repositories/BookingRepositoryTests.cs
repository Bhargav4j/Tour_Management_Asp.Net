using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveBookings()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.AddRange(
            new Booking { Id = 1, Email = "active@test.com", IsActive = true },
            new Booking { Id = 2, Email = "inactive@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenExists()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.Add(new Booking { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task AddAsync_AddsBookingToDatabase()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { Email = "new@test.com", FirstName = "Test" };

        var result = await repository.AddAsync(booking);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Bookings.CountAsync());
    }

    [Fact]
    public async Task GetByUserEmailAsync_ReturnsMatchingBookings()
    {
        using var context = new TourManagementDbContext(_options);
        context.Bookings.AddRange(
            new Booking { Email = "user@test.com", IsActive = true },
            new Booking { Email = "other@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var result = await repository.GetByUserEmailAsync("user@test.com");

        Assert.Single(result);
        Assert.Equal("user@test.com", result.First().Email);
    }
}
