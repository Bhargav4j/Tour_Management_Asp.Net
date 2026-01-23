using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbContextOptions;

    public BookingRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _dbContextOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true },
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 1, TotalAmount = 1000, IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result.First().NumberOfPersons);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBooking_WhenBookingExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(booking.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.NumberOfPersons);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true },
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 1, TotalAmount = 1000, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByUserIdAsync(user.Id);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsTourBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Bookings.AddRange(
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true },
            new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 1, TotalAmount = 1000, IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByTourIdAsync(tour.Id);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsBookingSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBookingSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        booking.NumberOfPersons = 3;
        await repository.UpdateAsync(booking);

        // Assert
        var updated = await context.Bookings.FindAsync(booking.Id);
        Assert.Equal(3, updated?.NumberOfPersons);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(booking.Id);

        // Assert
        var deleted = await context.Bookings.FindAsync(booking.Id);
        Assert.False(deleted?.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenBookingExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new BookingRepository(context, _mockLogger.Object);

        var user = new User { Email = "user@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking { UserId = user.Id, TourId = tour.Id, BookingDate = DateTime.Now, NumberOfPersons = 2, TotalAmount = 2000, IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(booking.Id);

        // Assert
        Assert.True(result);
    }
}
