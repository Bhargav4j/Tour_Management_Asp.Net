using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests.Data;

public class TourManagementDbContextTests
{
    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        // Act
        var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Users_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);

        // Act
        var usersDbSet = context.Users;

        // Assert
        Assert.NotNull(usersDbSet);
    }

    [Fact]
    public void Tours_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);

        // Act
        var toursDbSet = context.Tours;

        // Assert
        Assert.NotNull(toursDbSet);
    }

    [Fact]
    public void Bookings_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);

        // Act
        var bookingsDbSet = context.Bookings;

        // Assert
        Assert.NotNull(bookingsDbSet);
    }

    [Fact]
    public async Task Users_CanAddAndRetrieve()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var retrievedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("test@test.com", retrievedUser.Email);
    }

    [Fact]
    public async Task Tours_CanAddAndRetrieve()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000m,
            IsActive = true
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var retrievedTour = await context.Tours.FirstOrDefaultAsync(t => t.TourName == "Test Tour");

        // Assert
        Assert.NotNull(retrievedTour);
        Assert.Equal("Test Tour", retrievedTour.TourName);
    }

    [Fact]
    public async Task Bookings_CanAddAndRetrieve()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { TourName = "Test Tour", Place = "Test", IsActive = true };
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = user.Id,
            TourId = tour.Id,
            User = user,
            Tour = tour,
            NumberOfPeople = 2,
            TotalAmount = 1000m,
            Status = "Confirmed",
            IsActive = true
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
        var retrievedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.UserId == user.Id);

        // Assert
        Assert.NotNull(retrievedBooking);
        Assert.Equal(user.Id, retrievedBooking.UserId);
    }

    [Fact]
    public async Task Context_CanSaveChanges()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@test.com", FirstName = "Test", IsActive = true };

        // Act
        context.Users.Add(user);
        var result = await context.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        Assert.True(user.Id > 0);
    }

    [Fact]
    public async Task Context_CanHandleMultipleEntities()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { TourName = "Test Tour", Place = "Test", IsActive = true };

        // Act
        context.Users.Add(user);
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(user.Id > 0);
        Assert.True(tour.Id > 0);
        Assert.Single(await context.Users.ToListAsync());
        Assert.Single(await context.Tours.ToListAsync());
    }

    [Fact]
    public async Task Context_CanUpdateEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@test.com", FirstName = "Old", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.FirstName = "Updated";
        context.Users.Update(user);
        await context.SaveChangesAsync();

        // Assert
        var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task Context_CanDeleteEntity()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@test.com", FirstName = "Test", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        context.Users.Remove(user);
        await context.SaveChangesAsync();

        // Assert
        var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@test.com");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task Context_SupportsLinqQueries()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new TourManagementDbContext(options);
        var users = new List<User>
        {
            new User { Email = "user1@test.com", FirstName = "User1", IsActive = true },
            new User { Email = "user2@test.com", FirstName = "User2", IsActive = true },
            new User { Email = "user3@test.com", FirstName = "User3", IsActive = false }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Act
        var activeUsers = await context.Users.Where(u => u.IsActive).ToListAsync();

        // Assert
        Assert.Equal(2, activeUsers.Count);
        Assert.All(activeUsers, u => Assert.True(u.IsActive));
    }
}
