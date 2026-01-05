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

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.AddRange(
            new Booking { BookingId = 1, Email = "user1@test.com", IsActive = true, BookingDate = DateTime.UtcNow },
            new Booking { BookingId = 2, Email = "user2@test.com", IsActive = true, BookingDate = DateTime.UtcNow.AddDays(-1) },
            new Booking { BookingId = 3, Email = "user3@test.com", IsActive = false, BookingDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByBookingDateDescending()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var date1 = DateTime.UtcNow.AddDays(-2);
        var date2 = DateTime.UtcNow.AddDays(-1);
        var date3 = DateTime.UtcNow;
        context.Bookings.AddRange(
            new Booking { BookingId = 1, Email = "user1@test.com", IsActive = true, BookingDate = date1 },
            new Booking { BookingId = 2, Email = "user2@test.com", IsActive = true, BookingDate = date3 },
            new Booking { BookingId = 3, Email = "user3@test.com", IsActive = true, BookingDate = date2 }
        );
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result[0].BookingId);
        Assert.Equal(3, result[1].BookingId);
        Assert.Equal(1, result[2].BookingId);
    }

    [Fact]
    public async Task GetAllAsync_WithNoBookings_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var booking = new Booking { BookingId = 1, Email = "test@example.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BookingId);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.Add(new Booking { BookingId = 1, Email = "test@example.com", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithValidEmail_ShouldReturnUserBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.AddRange(
            new Booking { BookingId = 1, Email = "test@example.com", IsActive = true, BookingDate = DateTime.UtcNow },
            new Booking { BookingId = 2, Email = "test@example.com", IsActive = true, BookingDate = DateTime.UtcNow.AddDays(-1) },
            new Booking { BookingId = 3, Email = "other@example.com", IsActive = true, BookingDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal("test@example.com", b.Email));
    }

    [Fact]
    public async Task GetByUserEmailAsync_ShouldOrderByBookingDateDescending()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var date1 = DateTime.UtcNow.AddDays(-2);
        var date2 = DateTime.UtcNow;
        context.Bookings.AddRange(
            new Booking { BookingId = 1, Email = "test@example.com", IsActive = true, BookingDate = date1 },
            new Booking { BookingId = 2, Email = "test@example.com", IsActive = true, BookingDate = date2 }
        );
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetByUserEmailAsync("test@example.com")).ToList();

        // Assert
        Assert.Equal(2, result[0].BookingId);
        Assert.Equal(1, result[1].BookingId);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithNoBookings_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("nonexistent@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ShouldNotReturnInactiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.AddRange(
            new Booking { BookingId = 1, Email = "test@example.com", IsActive = true, BookingDate = DateTime.UtcNow },
            new Booking { BookingId = 2, Email = "test@example.com", IsActive = false, BookingDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.Single(result);
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task AddAsync_WithValidBooking_ShouldAddAndReturnBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { Email = "new@example.com", TourId = 1 };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.BookingId);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { Email = "new@example.com", TourId = 1 };

        // Act
        await repository.AddAsync(booking);

        // Assert
        var savedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.Email == "new@example.com");
        Assert.NotNull(savedBooking);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingBooking_ShouldUpdateBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var booking = new Booking { BookingId = 1, Email = "test@example.com", Status = "Pending", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        context.Entry(booking).State = EntityState.Detached;

        var repository = new BookingRepository(context, _mockLogger.Object);
        booking.Status = "Confirmed";

        // Act
        await repository.UpdateAsync(booking);

        // Assert
        var updatedBooking = await context.Bookings.FindAsync(1);
        Assert.Equal("Confirmed", updatedBooking!.Status);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingBooking_ShouldSetIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var booking = new Booking { BookingId = 1, Email = "test@example.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(1);
        Assert.NotNull(deletedBooking);
        Assert.False(deletedBooking.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var booking = new Booking { BookingId = 1, Email = "test@example.com", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(1);
        Assert.NotNull(deletedBooking!.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingBooking_ShouldNotThrowException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        // No exception should be thrown
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveBooking_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.Add(new Booking { BookingId = 1, Email = "test@example.com", IsActive = true });
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingBooking_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Bookings.Add(new Booking { BookingId = 1, Email = "test@example.com", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }
}
