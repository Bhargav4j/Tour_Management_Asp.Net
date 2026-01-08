using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private DbContextOptions<TourManagementDbContext> CreateNewContextOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Arrange
        var options = CreateNewContextOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Tours_Property_ReturnsDbSet()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var tours = context.Tours;

        // Assert
        Assert.NotNull(tours);
    }

    [Fact]
    public void Users_Property_ReturnsDbSet()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public void Bookings_Property_ReturnsDbSet()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var bookings = context.Bookings;

        // Assert
        Assert.NotNull(bookings);
    }

    [Fact]
    public async Task Tours_CanAddAndRetrieveTour()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour
        {
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 7,
            Price = 1999.99m,
            IsActive = true
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var retrievedTour = await context.Tours.FirstOrDefaultAsync(t => t.TourName == "Paris Tour");
        Assert.NotNull(retrievedTour);
        Assert.Equal("Paris Tour", retrievedTour.TourName);
        Assert.Equal("Paris", retrievedTour.Place);
    }

    [Fact]
    public async Task Users_CanAddAndRetrieveUser()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var retrievedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(retrievedUser);
        Assert.Equal("test@example.com", retrievedUser.Email);
        Assert.Equal("John", retrievedUser.FirstName);
    }

    [Fact]
    public async Task Bookings_CanAddAndRetrieveBooking()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking
        {
            TourName = "Paris Tour",
            Email = "test@example.com",
            FirstName = "John",
            IsActive = true
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var retrievedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.Email == "test@example.com");
        Assert.NotNull(retrievedBooking);
        Assert.Equal("Paris Tour", retrievedBooking.TourName);
        Assert.Equal("test@example.com", retrievedBooking.Email);
    }

    [Fact]
    public async Task Tours_CanUpdateTour()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { TourName = "Original Name", Place = "Paris" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated Name";
        await context.SaveChangesAsync();

        // Assert
        var updatedTour = await context.Tours.FirstOrDefaultAsync(t => t.Id == tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Name", updatedTour.TourName);
    }

    [Fact]
    public async Task Users_CanUpdateUser()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Email = "old@example.com", FirstName = "John" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Email = "new@example.com";
        await context.SaveChangesAsync();

        // Assert
        var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("new@example.com", updatedUser.Email);
    }

    [Fact]
    public async Task Bookings_CanUpdateBooking()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { TourName = "Old Tour", Email = "test@example.com" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        booking.TourName = "New Tour";
        await context.SaveChangesAsync();

        // Assert
        var updatedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == booking.Id);
        Assert.NotNull(updatedBooking);
        Assert.Equal("New Tour", updatedBooking.TourName);
    }

    [Fact]
    public async Task Tours_CanDeleteTour()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var tourId = tour.Id;

        // Act
        context.Tours.Remove(tour);
        await context.SaveChangesAsync();

        // Assert
        var deletedTour = await context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);
        Assert.Null(deletedTour);
    }

    [Fact]
    public async Task Users_CanDeleteUser()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@example.com", FirstName = "John" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var userId = user.Id;

        // Act
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // Assert
        var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Bookings_CanDeleteBooking()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var booking = new Booking { TourName = "Paris Tour", Email = "test@example.com" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var bookingId = booking.Id;

        // Act
        context.Bookings.Remove(booking);
        await context.SaveChangesAsync();

        // Assert
        var deletedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        Assert.Null(deletedBooking);
    }

    [Fact]
    public async Task Context_CanSaveMultipleEntities()
    {
        // Arrange
        var options = CreateNewContextOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris" };
        var user = new User { Email = "test@example.com", FirstName = "John" };
        var booking = new Booking { TourName = "Paris Tour", Email = "test@example.com" };

        // Act
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(tour.Id > 0);
        Assert.True(user.Id > 0);
        Assert.True(booking.Id > 0);
    }
}
