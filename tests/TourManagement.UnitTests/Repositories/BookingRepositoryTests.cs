using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Repositories;

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
    public async Task GetAllAsync_ShouldReturnOnlyActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Confirmed", IsActive = true, CreatedBy = "System" },
            new Booking { Id = 2, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000, Status = "Pending", IsActive = false, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Confirmed", result.First().Status);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBooking_WhenBookingExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Paris Tour", IsActive = true, Place = "Paris", Days = 7, Price = 1500, Locations = "Eiffel", TourInfo = "Great", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 4, TotalAmount = 6000, Status = "Confirmed", IsActive = true, CreatedBy = "System" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(booking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.NumberOfPeople);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookingDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Confirmed", IsActive = true, CreatedBy = "System" },
            new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000, Status = "Pending", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldReturnTourBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Confirmed", IsActive = true, CreatedBy = "System" },
            new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 1, TotalAmount = 1000, Status = "Pending", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_ShouldAddBooking_AndReturnBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000, Status = "Pending", IsActive = true, CreatedBy = "System" };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal(3, result.NumberOfPeople);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Pending", IsActive = true, CreatedBy = "System" };
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
    public async Task DeleteAsync_ShouldSoftDeleteBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Confirmed", IsActive = true, CreatedBy = "System" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(booking.Id);

        // Assert
        var deleted = await context.Bookings.FindAsync(booking.Id);
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenBookingExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "user@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St", City = "City", State = "ST", CreatedBy = "System" };
        var tour = new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", CreatedBy = "System" };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000, Status = "Pending", IsActive = true, CreatedBy = "System" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _mockLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(context, null!));
    }
}
