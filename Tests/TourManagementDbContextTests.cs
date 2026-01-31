using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public TourManagementDbContextTests()
    {
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private TourManagementDbContext CreateContext()
    {
        return new TourManagementDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Act
        using var context = CreateContext();

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.Users);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public void Tours_DbSet_IsAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var tours = context.Tours;

        // Assert
        Assert.NotNull(tours);
    }

    [Fact]
    public void Users_DbSet_IsAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public void Bookings_DbSet_IsAccessible()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var bookings = context.Bookings;

        // Assert
        Assert.NotNull(bookings);
    }

    [Fact]
    public async Task Tours_CanAddAndRetrieve()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 7, Price = 1999m };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var savedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(savedTour);
        Assert.Equal("Paris Tour", savedTour.TourName);
    }

    [Fact]
    public async Task Users_CanAddAndRetrieve()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User { Username = "john_doe", Email = "john@test.com", PasswordHash = "hash123" };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("john_doe", savedUser.Username);
    }

    [Fact]
    public async Task Bookings_CanAddAndRetrieve()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, UserId = user.Id, NumberOfPeople = 2, TotalAmount = 1000m };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var savedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(savedBooking);
        Assert.Equal(2, savedBooking.NumberOfPeople);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleEntities_SavesAllEntities()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        var booking = new Booking { Tour = tour, User = user, NumberOfPeople = 2 };

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

    [Fact]
    public async Task Context_SupportsTransactions()
    {
        // Arrange
        using var context = CreateContext();
        var tour1 = new Tour { TourName = "Tour1", IsActive = true };
        var tour2 = new Tour { TourName = "Tour2", IsActive = true };

        // Act
        using var transaction = await context.Database.BeginTransactionAsync();
        context.Tours.Add(tour1);
        await context.SaveChangesAsync();

        context.Tours.Add(tour2);
        await context.SaveChangesAsync();

        await transaction.CommitAsync();

        // Assert
        var tours = await context.Tours.ToListAsync();
        Assert.Equal(2, tours.Count);
    }

    [Fact]
    public async Task Context_SupportsRollback()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };

        // Act
        using var transaction = await context.Database.BeginTransactionAsync();
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        await transaction.RollbackAsync();

        // Assert
        var tours = await context.Tours.ToListAsync();
        Assert.Empty(tours);
    }

    [Fact]
    public async Task Tour_WithBookings_CanLoadNavigationProperty()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, UserId = user.Id, NumberOfPeople = 2 };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var loadedTour = await context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == tour.Id);

        // Assert
        Assert.NotNull(loadedTour);
        Assert.NotEmpty(loadedTour.Bookings);
        Assert.Single(loadedTour.Bookings);
    }

    [Fact]
    public async Task User_WithBookings_CanLoadNavigationProperty()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, UserId = user.Id, NumberOfPeople = 2 };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var loadedUser = await context.Users
            .Include(u => u.Bookings)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.NotNull(loadedUser);
        Assert.NotEmpty(loadedUser.Bookings);
        Assert.Single(loadedUser.Bookings);
    }

    [Fact]
    public async Task Booking_WithTourAndUser_CanLoadNavigationProperties()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Tours.Add(tour);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var booking = new Booking { TourId = tour.Id, UserId = user.Id, NumberOfPeople = 2 };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Act
        var loadedBooking = await context.Bookings
            .Include(b => b.Tour)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == booking.Id);

        // Assert
        Assert.NotNull(loadedBooking);
        Assert.NotNull(loadedBooking.Tour);
        Assert.NotNull(loadedBooking.User);
        Assert.Equal("Tour1", loadedBooking.Tour.TourName);
        Assert.Equal("user1", loadedBooking.User.Username);
    }
}
