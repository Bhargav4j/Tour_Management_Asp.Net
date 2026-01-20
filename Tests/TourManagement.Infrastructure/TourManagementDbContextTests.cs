using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _dbContextOptions;

    public TourManagementDbContextTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void TourManagementDbContext_Constructor_CreatesContext()
    {
        // Act
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void TourManagementDbContext_ToursDbSet_IsNotNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Act & Assert
        Assert.NotNull(context.Tours);
    }

    [Fact]
    public void TourManagementDbContext_UserInfosDbSet_IsNotNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Act & Assert
        Assert.NotNull(context.UserInfos);
    }

    [Fact]
    public void TourManagementDbContext_BookingsDbSet_IsNotNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Act & Assert
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task TourManagementDbContext_CanAddTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var tour = new Tour { TourName = "Test Tour", Price = 1000m };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var savedTour = await context.Tours.FirstOrDefaultAsync(t => t.TourName == "Test Tour");
        Assert.NotNull(savedTour);
        Assert.Equal("Test Tour", savedTour.TourName);
    }

    [Fact]
    public async Task TourManagementDbContext_CanAddUserInfo()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var user = new UserInfo { Email = "test@example.com", FirstName = "John", LastName = "Doe" };

        // Act
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.UserInfos.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(savedUser);
        Assert.Equal("test@example.com", savedUser.Email);
    }

    [Fact]
    public async Task TourManagementDbContext_CanAddBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var savedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.UserId == 1);
        Assert.NotNull(savedBooking);
        Assert.Equal(1, savedBooking.UserId);
    }

    [Fact]
    public async Task TourManagementDbContext_CanUpdateTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var tour = new Tour { TourName = "Old Name", Price = 1000m };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "New Name";
        await context.SaveChangesAsync();

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("New Name", updatedTour.TourName);
    }

    [Fact]
    public async Task TourManagementDbContext_CanDeleteTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var tour = new Tour { TourName = "Tour to Delete", Price = 1000m };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        context.Tours.Remove(tour);
        await context.SaveChangesAsync();

        // Assert
        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.Null(deletedTour);
    }

    [Fact]
    public async Task TourManagementDbContext_CanQueryTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Price = 1000m },
            new Tour { TourName = "Tour 2", Price = 2000m },
            new Tour { TourName = "Tour 3", Price = 3000m }
        );
        await context.SaveChangesAsync();

        // Act
        var tours = await context.Tours.ToListAsync();

        // Assert
        Assert.Equal(3, tours.Count);
    }

    [Fact]
    public async Task TourManagementDbContext_CanQueryUserInfos()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        context.UserInfos.AddRange(
            new UserInfo { Email = "user1@test.com", FirstName = "User1" },
            new UserInfo { Email = "user2@test.com", FirstName = "User2" }
        );
        await context.SaveChangesAsync();

        // Act
        var users = await context.UserInfos.ToListAsync();

        // Assert
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task TourManagementDbContext_CanQueryBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        context.Bookings.AddRange(
            new Booking { UserId = 1, TourId = 1 },
            new Booking { UserId = 2, TourId = 2 },
            new Booking { UserId = 3, TourId = 3 }
        );
        await context.SaveChangesAsync();

        // Act
        var bookings = await context.Bookings.ToListAsync();

        // Assert
        Assert.Equal(3, bookings.Count);
    }

    [Fact]
    public async Task TourManagementDbContext_SaveChangesAsync_ReturnsAffectedRows()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var tour = new Tour { TourName = "Test Tour", Price = 1000m };
        context.Tours.Add(tour);

        // Act
        var result = await context.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
    }
}
