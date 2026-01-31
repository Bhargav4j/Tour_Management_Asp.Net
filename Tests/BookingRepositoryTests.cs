using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private TourManagementDbContext CreateContext()
    {
        return new TourManagementDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 2, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 3, TourId = 1, UserId = 1, IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.DoesNotContain(result, b => b.Id == 3);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ReturnsUserBookings()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user1 = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        var user2 = new User { Id = 2, Username = "user2", Email = "user2@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.AddRange(user1, user2);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 2, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 3, TourId = 1, UserId = 2, IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
    }

    [Fact]
    public async Task GetByTourIdAsync_WithValidTourId_ReturnsTourBookings()
    {
        // Arrange
        using var context = CreateContext();
        var tour1 = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Tour2", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.AddRange(tour1, tour2);
        context.Users.Add(user);
        context.Bookings.AddRange(
            new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 2, TourId = 1, UserId = 1, IsActive = true },
            new Booking { Id = 3, TourId = 2, UserId = 1, IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
    }

    [Fact]
    public async Task AddAsync_AddsBookingToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2, TotalAmount = 1000m };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(2, result.NumberOfPeople);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBookingInDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        var booking = new Booking { TourId = 1, UserId = 1, NumberOfPeople = 2, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);
        booking.NumberOfPeople = 5;

        // Act
        var result = await repository.UpdateAsync(booking);

        // Assert
        Assert.Equal(5, result.NumberOfPeople);
        var updatedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.Equal(5, updatedBooking?.NumberOfPeople);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        var booking = new Booking { TourId = 1, UserId = 1, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(booking.Id);

        // Assert
        Assert.True(result);
        var deletedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.False(deletedBooking?.IsActive);
        Assert.NotNull(deletedBooking?.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveBooking_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = false });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_IncludesTourAndUser()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var user = new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(new Booking { Id = 1, TourId = 1, UserId = 1, IsActive = true });
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        var booking = result.First();
        Assert.NotNull(booking.Tour);
        Assert.NotNull(booking.User);
        Assert.Equal("Tour1", booking.Tour.TourName);
        Assert.Equal("user1", booking.User.Username);
    }
}
