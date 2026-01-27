using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;

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
    public void BookingRepository_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void BookingRepository_Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void BookingRepository_Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking1 = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
        var booking2 = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow.AddDays(-1), NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) };
        var booking3 = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 1, TotalAmount = 1000m, Status = "Cancelled", IsActive = false, CreatedDate = DateTime.UtcNow };

        context.Bookings.AddRange(booking1, booking2, booking3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(booking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
        Assert.Equal(2, result.NumberOfPeople);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
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
    public async Task GetByUserIdAsync_ReturnsUserBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user1 = new User { Email = "user1@test.com", FirstName = "User1", LastName = "Test", IsActive = true, CreatedDate = DateTime.UtcNow };
        var user2 = new User { Email = "user2@test.com", FirstName = "User2", LastName = "Test", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.AddRange(user1, user2);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking1 = new Booking { UserId = user1.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
        var booking2 = new Booking { UserId = user1.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow };
        var booking3 = new Booking { UserId = user2.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 1, TotalAmount = 1000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };

        context.Bookings.AddRange(booking1, booking2, booking3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(user1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(user1.Id, b.UserId));
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsTourBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour1 = new Tour { TourName = "Tour1", Place = "Place1", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour2 = new Tour { TourName = "Tour2", Place = "Place2", Days = 6, Price = 1200m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.AddRange(tour1, tour2);
        await context.SaveChangesAsync();

        var booking1 = new Booking { UserId = user.Id, TourId = tour1.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
        var booking2 = new Booking { UserId = user.Id, TourId = tour1.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow };
        var booking3 = new Booking { UserId = user.Id, TourId = tour2.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 1, TotalAmount = 1000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };

        context.Bookings.AddRange(booking1, booking2, booking3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(tour1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(tour1.Id, b.TourId));
    }

    [Fact]
    public async Task AddAsync_AddsBookingToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = user.Id,
            TourId = tour.Id,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 4,
            TotalAmount = 4000m,
            Status = "Confirmed",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(4, result.NumberOfPeople);

        var savedBooking = await context.Bookings.FindAsync(result.Id);
        Assert.NotNull(savedBooking);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        context.Entry(booking).State = EntityState.Detached;

        // Act
        booking.Status = "Confirmed";
        await repository.UpdateAsync(booking);

        // Assert
        var updatedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(updatedBooking);
        Assert.Equal("Confirmed", updatedBooking.Status);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
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
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingBooking_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingBooking_ReturnsFalse()
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
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
