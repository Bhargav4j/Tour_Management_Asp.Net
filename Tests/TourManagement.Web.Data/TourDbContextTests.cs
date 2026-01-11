using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;
using TourManagement.Web.Models;
using System;

namespace TourManagement.Web.Data.Tests;

public class TourDbContextTests
{
    private DbContextOptions<TourDbContext> CreateInMemoryOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<TourDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
    }

    [Fact]
    public void TourDbContext_Constructor_InitializesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_Constructor");

        // Act
        using var context = new TourDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void TourDbContext_UserInfos_DbSet_IsNotNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_UserInfos");

        // Act
        using var context = new TourDbContext(options);

        // Assert
        Assert.NotNull(context.UserInfos);
    }

    [Fact]
    public void TourDbContext_Tours_DbSet_IsNotNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_Tours");

        // Act
        using var context = new TourDbContext(options);

        // Assert
        Assert.NotNull(context.Tours);
    }

    [Fact]
    public void TourDbContext_Bookings_DbSet_IsNotNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_Bookings");

        // Act
        using var context = new TourDbContext(options);

        // Assert
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task TourDbContext_AddUserInfo_SavesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_AddUserInfo");
        using var context = new TourDbContext(options);

        var userInfo = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateOnly(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        context.UserInfos.Add(userInfo);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.UserInfos.FindAsync("test@example.com");
        Assert.NotNull(savedUser);
        Assert.Equal("John", savedUser.FirstName);
    }

    [Fact]
    public async Task TourDbContext_AddTour_SavesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_AddTour");
        using var context = new TourDbContext(options);

        var tour = new Tour
        {
            TourName = "Europe Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500.00m,
            Locations = "Paris, London",
            TourInfo = "Amazing tour"
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var savedTour = await context.Tours.FirstOrDefaultAsync();
        Assert.NotNull(savedTour);
        Assert.Equal("Europe Tour", savedTour.TourName);
    }

    [Fact]
    public async Task TourDbContext_AddBooking_SavesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_AddBooking");
        using var context = new TourDbContext(options);

        var booking = new Booking
        {
            TourName = "Asia Tour",
            Place = "Tokyo",
            Email = "customer@example.com",
            FirstName = "Alice"
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var savedBooking = await context.Bookings.FirstOrDefaultAsync();
        Assert.NotNull(savedBooking);
        Assert.Equal("Asia Tour", savedBooking.TourName);
    }

    [Fact]
    public async Task TourDbContext_UpdateUserInfo_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_UpdateUserInfo");
        using var context = new TourDbContext(options);

        var userInfo = new UserInfo
        {
            Email = "update@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "pass456",
            Dob = new DateOnly(1985, 5, 15),
            Street = "456 Oak St",
            City = "Boston",
            State = "MA"
        };

        context.UserInfos.Add(userInfo);
        await context.SaveChangesAsync();

        // Act
        userInfo.FirstName = "Janet";
        await context.SaveChangesAsync();

        // Assert
        var updatedUser = await context.UserInfos.FindAsync("update@example.com");
        Assert.NotNull(updatedUser);
        Assert.Equal("Janet", updatedUser.FirstName);
    }

    [Fact]
    public async Task TourDbContext_DeleteUserInfo_DeletesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_DeleteUserInfo");
        using var context = new TourDbContext(options);

        var userInfo = new UserInfo
        {
            Email = "delete@example.com",
            FirstName = "Bob",
            LastName = "Jones",
            Gender = "Male",
            Password = "pass789",
            Dob = new DateOnly(1980, 10, 10),
            Street = "789 Pine St",
            City = "Chicago",
            State = "IL"
        };

        context.UserInfos.Add(userInfo);
        await context.SaveChangesAsync();

        // Act
        context.UserInfos.Remove(userInfo);
        await context.SaveChangesAsync();

        // Assert
        var deletedUser = await context.UserInfos.FindAsync("delete@example.com");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task TourDbContext_QueryUserInfos_ReturnsMultipleRecords()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_QueryMultipleUsers");
        using var context = new TourDbContext(options);

        context.UserInfos.AddRange(
            new UserInfo
            {
                Email = "user1@example.com",
                FirstName = "User1",
                LastName = "Last1",
                Gender = "Male",
                Password = "pass1",
                Dob = new DateOnly(1990, 1, 1),
                Street = "Street1",
                City = "City1",
                State = "State1"
            },
            new UserInfo
            {
                Email = "user2@example.com",
                FirstName = "User2",
                LastName = "Last2",
                Gender = "Female",
                Password = "pass2",
                Dob = new DateOnly(1991, 2, 2),
                Street = "Street2",
                City = "City2",
                State = "State2"
            }
        );
        await context.SaveChangesAsync();

        // Act
        var users = await context.UserInfos.ToListAsync();

        // Assert
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task TourDbContext_QueryTours_ReturnsMultipleRecords()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_QueryMultipleTours");
        using var context = new TourDbContext(options);

        context.Tours.AddRange(
            new Tour
            {
                TourName = "Tour1",
                Place = "Place1",
                Days = 5,
                Price = 1000m,
                Locations = "Loc1",
                TourInfo = "Info1"
            },
            new Tour
            {
                TourName = "Tour2",
                Place = "Place2",
                Days = 10,
                Price = 2000m,
                Locations = "Loc2",
                TourInfo = "Info2"
            }
        );
        await context.SaveChangesAsync();

        // Act
        var tours = await context.Tours.ToListAsync();

        // Assert
        Assert.Equal(2, tours.Count);
    }

    [Fact]
    public async Task TourDbContext_QueryBookings_ReturnsMultipleRecords()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_QueryMultipleBookings");
        using var context = new TourDbContext(options);

        context.Bookings.AddRange(
            new Booking
            {
                TourName = "Booking1",
                Place = "Place1",
                Email = "email1@test.com",
                FirstName = "Name1"
            },
            new Booking
            {
                TourName = "Booking2",
                Place = "Place2",
                Email = "email2@test.com",
                FirstName = "Name2"
            }
        );
        await context.SaveChangesAsync();

        // Act
        var bookings = await context.Bookings.ToListAsync();

        // Assert
        Assert.Equal(2, bookings.Count);
    }

    [Fact]
    public async Task TourDbContext_FindUserInfoByEmail_ReturnsCorrectUser()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_FindUserByEmail");
        using var context = new TourDbContext(options);

        var userInfo = new UserInfo
        {
            Email = "find@example.com",
            FirstName = "Find",
            LastName = "Me",
            Gender = "Male",
            Password = "findpass",
            Dob = new DateOnly(1995, 3, 3),
            Street = "Find St",
            City = "Find City",
            State = "FS"
        };

        context.UserInfos.Add(userInfo);
        await context.SaveChangesAsync();

        // Act
        var foundUser = await context.UserInfos.FindAsync("find@example.com");

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal("Find", foundUser.FirstName);
    }

    [Fact]
    public async Task TourDbContext_UpdateTour_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_UpdateTour");
        using var context = new TourDbContext(options);

        var tour = new Tour
        {
            TourName = "Old Tour",
            Place = "Old Place",
            Days = 3,
            Price = 500m,
            Locations = "Old Loc",
            TourInfo = "Old Info"
        };

        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "New Tour";
        await context.SaveChangesAsync();

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.TourId);
        Assert.NotNull(updatedTour);
        Assert.Equal("New Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task TourDbContext_DeleteTour_DeletesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_DeleteTour");
        using var context = new TourDbContext(options);

        var tour = new Tour
        {
            TourName = "Delete Tour",
            Place = "Delete Place",
            Days = 4,
            Price = 600m,
            Locations = "Delete Loc",
            TourInfo = "Delete Info"
        };

        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var tourId = tour.TourId;

        // Act
        context.Tours.Remove(tour);
        await context.SaveChangesAsync();

        // Assert
        var deletedTour = await context.Tours.FindAsync(tourId);
        Assert.Null(deletedTour);
    }

    [Fact]
    public async Task TourDbContext_SaveChangesAsync_ReturnsNumberOfAffectedEntries()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_SaveChangesCount");
        using var context = new TourDbContext(options);

        var tour = new Tour
        {
            TourName = "Count Tour",
            Place = "Count Place",
            Days = 2,
            Price = 400m,
            Locations = "Count Loc",
            TourInfo = "Count Info"
        };

        context.Tours.Add(tour);

        // Act
        var affectedRows = await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, affectedRows);
    }

    [Fact]
    public void TourDbContext_Model_ContainsUserInfoEntity()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_ModelUserInfo");
        using var context = new TourDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(UserInfo));

        // Assert
        Assert.NotNull(entityType);
    }

    [Fact]
    public void TourDbContext_Model_ContainsTourEntity()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_ModelTour");
        using var context = new TourDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Assert
        Assert.NotNull(entityType);
    }

    [Fact]
    public void TourDbContext_Model_ContainsBookingEntity()
    {
        // Arrange
        var options = CreateInMemoryOptions("TestDb_ModelBooking");
        using var context = new TourDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        Assert.NotNull(entityType);
    }
}
