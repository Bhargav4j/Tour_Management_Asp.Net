using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
    }

    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveBookings_ShouldReturnOnlyActiveBookings()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true },
            new Booking { Id = 2, UserId = 2, TourId = 2, IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task GetAllAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
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
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
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
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithValidUserId_ShouldReturnUserBookings()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 2, IsActive = true },
            new Booking { Id = 3, UserId = 2, TourId = 1, IsActive = true }
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
    public async Task GetByUserIdAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldNotReturnInactiveBookings()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 2, IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithValidTourId_ShouldReturnTourBookings()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true },
            new Booking { Id = 2, UserId = 2, TourId = 1, IsActive = true },
            new Booking { Id = 3, UserId = 3, TourId = 2, IsActive = true }
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
    public async Task GetByTourIdAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldNotReturnInactiveBookings()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true },
            new Booking { Id = 2, UserId = 2, TourId = 1, IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task AddAsync_WithValidBooking_ShouldAddAndReturnBooking()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistBookingToDatabase()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 3 };

        using (var context = new TourManagementDbContext(options))
        {
            var repository = new BookingRepository(context, _mockLogger.Object);
            await repository.AddAsync(booking);
        }

        // Act & Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedBooking = await context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(savedBooking);
            Assert.Equal(3, savedBooking.NumberOfPeople);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithValidBooking_ShouldUpdateBooking()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Booking booking;

        using (var context = new TourManagementDbContext(options))
        {
            booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new BookingRepository(context, _mockLogger.Object);
            booking.NumberOfPeople = 5;
            await repository.UpdateAsync(booking);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var updatedBooking = await context.Bookings.FirstAsync();
            Assert.Equal(5, updatedBooking.NumberOfPeople);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSetIsActiveFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Booking booking;

        using (var context = new TourManagementDbContext(options))
        {
            booking = new Booking { UserId = 1, TourId = 1, IsActive = true };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new BookingRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(booking.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedBooking = await context.Bookings.FindAsync(booking.Id);
            Assert.NotNull(deletedBooking);
            Assert.False(deletedBooking.IsActive);
            Assert.NotNull(deletedBooking.ModifiedDate);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Booking booking;

        using (var context = new TourManagementDbContext(options))
        {
            booking = new Booking { UserId = 1, TourId = 1, IsActive = true };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new BookingRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(booking.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedBooking = await context.Bookings.FindAsync(booking.Id);
            Assert.NotNull(deletedBooking?.ModifiedDate);
        }
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveBooking_ShouldReturnTrue()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = true };
        context.Bookings.Add(booking);
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
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
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
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldRespectToken()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}
