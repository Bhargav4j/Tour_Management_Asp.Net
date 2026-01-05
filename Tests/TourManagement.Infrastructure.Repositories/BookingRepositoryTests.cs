using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _loggerMock;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public BookingRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<BookingRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);

        // Act
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnActiveBookings()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking1 = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        var booking2 = new Booking { BookingId = 2, UserId = 2, TourId = 2, NumberOfPeople = 3, TotalPrice = 300m, BookingDate = DateTime.Now, IsActive = true };
        var booking3 = new Booking { BookingId = 3, UserId = 3, TourId = 3, NumberOfPeople = 1, TotalPrice = 100m, BookingDate = DateTime.Now, IsActive = false };

        await context.Bookings.AddRangeAsync(booking1, booking2, booking3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BookingId);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = false };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking1 = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        var booking2 = new Booking { BookingId = 2, UserId = 1, TourId = 2, NumberOfPeople = 3, TotalPrice = 300m, BookingDate = DateTime.Now, IsActive = true };
        var booking3 = new Booking { BookingId = 3, UserId = 2, TourId = 3, NumberOfPeople = 1, TotalPrice = 100m, BookingDate = DateTime.Now, IsActive = true };

        await context.Bookings.AddRangeAsync(booking1, booking2, booking3);
        await context.SaveChangesAsync();

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
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldNotReturnInactiveBookings()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking1 = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        var booking2 = new Booking { BookingId = 2, UserId = 1, TourId = 2, NumberOfPeople = 3, TotalPrice = 300m, BookingDate = DateTime.Now, IsActive = false };

        await context.Bookings.AddRangeAsync(booking1, booking2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldReturnTourBookings()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking1 = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        var booking2 = new Booking { BookingId = 2, UserId = 2, TourId = 1, NumberOfPeople = 3, TotalPrice = 300m, BookingDate = DateTime.Now, IsActive = true };
        var booking3 = new Booking { BookingId = 3, UserId = 3, TourId = 2, NumberOfPeople = 1, TotalPrice = 100m, BookingDate = DateTime.Now, IsActive = true };

        await context.Bookings.AddRangeAsync(booking1, booking2, booking3);
        await context.SaveChangesAsync();

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
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldNotReturnInactiveBookings()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking1 = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        var booking2 = new Booking { BookingId = 2, UserId = 2, TourId = 1, NumberOfPeople = 3, TotalPrice = 300m, BookingDate = DateTime.Now, IsActive = false };

        await context.Bookings.AddRangeAsync(booking1, booking2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddBooking()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.BookingId > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBooking()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        booking.NumberOfPeople = 5;
        booking.TotalPrice = 500m;
        await repository.UpdateAsync(booking);

        // Assert
        var updatedBooking = await context.Bookings.FindAsync(booking.BookingId);
        Assert.NotNull(updatedBooking);
        Assert.Equal(5, updatedBooking.NumberOfPeople);
        Assert.Equal(500m, updatedBooking.TotalPrice);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkBookingAsInactive()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(booking.BookingId);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(booking.BookingId);
        Assert.NotNull(deletedBooking);
        Assert.False(deletedBooking.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = true };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.BookingId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m, BookingDate = DateTime.Now, IsActive = false };
        await context.Bookings.AddAsync(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.BookingId);

        // Assert
        Assert.False(result);
    }
}
