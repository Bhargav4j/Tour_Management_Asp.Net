using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourManagementDbContextTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldInitializeContext()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.UserInfos);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public void Tours_DbSet_ShouldBeAccessible()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);

        // Assert
        Assert.NotNull(context.Tours);
        Assert.IsAssignableFrom<DbSet<Tour>>(context.Tours);
    }

    [Fact]
    public void UserInfos_DbSet_ShouldBeAccessible()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);

        // Assert
        Assert.NotNull(context.UserInfos);
        Assert.IsAssignableFrom<DbSet<UserInfo>>(context.UserInfos);
    }

    [Fact]
    public void Bookings_DbSet_ShouldBeAccessible()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);

        // Assert
        Assert.NotNull(context.Bookings);
        Assert.IsAssignableFrom<DbSet<Booking>>(context.Bookings);
    }

    [Fact]
    public async Task AddTour_ShouldSaveTourToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000,
            Locations = "Eiffel Tower",
            TourInfo = "Great tour",
            IsActive = true,
            CreatedBy = "System"
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var savedTour = await context.Tours.FirstOrDefaultAsync(t => t.TourName == "Test Tour");
        Assert.NotNull(savedTour);
        Assert.Equal("Paris", savedTour.Place);
    }

    [Fact]
    public async Task AddUserInfo_ShouldSaveUserToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "hashed",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            IsActive = true,
            CreatedBy = "System"
        };

        // Act
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.UserInfos.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(savedUser);
        Assert.Equal("John", savedUser.FirstName);
    }

    [Fact]
    public async Task AddBooking_ShouldSaveBookingToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking
        {
            TourId = 1,
            TourName = "Test Tour",
            Place = "Paris",
            Email = "test@example.com",
            FirstName = "John",
            IsActive = true,
            CreatedBy = "System"
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var savedBooking = await context.Bookings.FirstOrDefaultAsync(b => b.Email == "test@example.com");
        Assert.NotNull(savedBooking);
        Assert.Equal("Test Tour", savedBooking.TourName);
    }

    [Fact]
    public void Model_ShouldHavePublicSchema()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var schema = context.Model.GetDefaultSchema();

        // Assert
        Assert.Equal("public", schema);
    }
}
