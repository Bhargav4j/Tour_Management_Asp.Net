using Microsoft.EntityFrameworkCore;
using Xunit;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Tours_DbSet_ShouldNotBeNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var tours = context.Tours;

        // Assert
        Assert.NotNull(tours);
    }

    [Fact]
    public void Users_DbSet_ShouldNotBeNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public void Bookings_DbSet_ShouldNotBeNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var bookings = context.Bookings;

        // Assert
        Assert.NotNull(bookings);
    }

    [Fact]
    public void Tours_AddEntity_ShouldSucceed()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);
        var tour = new Tour
        {
            Id = 1,
            Name = "Test Tour",
            Description = "Test Description",
            Price = 100.00m,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7),
            MaxParticipants = 10,
            AvailableSpots = 10
        };

        // Act
        context.Tours.Add(tour);
        context.SaveChanges();

        // Assert
        Assert.Equal(1, context.Tours.Count());
    }

    [Fact]
    public void Users_AddEntity_ShouldSucceed()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            FirstName = "Test",
            LastName = "User",
            CreatedAt = DateTime.Now
        };

        // Act
        context.Users.Add(user);
        context.SaveChanges();

        // Assert
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public void Bookings_AddEntity_ShouldSucceed()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            NumberOfParticipants = 2,
            TotalPrice = 200.00m,
            BookingDate = DateTime.Now,
            Status = "Confirmed"
        };

        // Act
        context.Bookings.Add(booking);
        context.SaveChanges();

        // Assert
        Assert.Equal(1, context.Bookings.Count());
    }

    [Fact]
    public void SaveChanges_ShouldPersistData()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour
        {
            Id = 1,
            Name = "Persist Test",
            Description = "Test",
            Price = 50.00m,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(3),
            MaxParticipants = 5,
            AvailableSpots = 5
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            Assert.Equal(1, context.Tours.Count());
            var savedTour = context.Tours.First();
            Assert.Equal("Persist Test", savedTour.Name);
        }
    }

    [Fact]
    public void OnModelCreating_ShouldApplyConfigurations()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var model = context.Model;

        // Assert
        Assert.NotNull(model);
        Assert.NotNull(model.FindEntityType(typeof(Tour)));
        Assert.NotNull(model.FindEntityType(typeof(User)));
        Assert.NotNull(model.FindEntityType(typeof(Booking)));
    }
}
