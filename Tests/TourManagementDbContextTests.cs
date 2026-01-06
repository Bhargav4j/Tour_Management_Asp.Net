using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_InitializesContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        // Act
        var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Tours_DbSetIsNotNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act
        var tours = context.Tours;

        // Assert
        Assert.NotNull(tours);
    }

    [Fact]
    public void Users_DbSetIsNotNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
    }

    [Fact]
    public void Bookings_DbSetIsNotNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act
        var bookings = context.Bookings;

        // Assert
        Assert.NotNull(bookings);
    }

    [Fact]
    public void Context_CanAddTour()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Test Tour", Place = "Paris" };

        // Act
        context.Tours.Add(tour);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Tours);
    }

    [Fact]
    public void Context_CanAddUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Email = "test@test.com", FirstName = "John" };

        // Act
        context.Users.Add(user);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Users);
    }

    [Fact]
    public void Context_CanAddBooking()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var booking = new Booking { UserId = 1, TourId = 1 };

        // Act
        context.Bookings.Add(booking);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Bookings);
    }

    [Fact]
    public void Context_CanQueryTours()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { TourName = "Tour 1" });
        context.Tours.Add(new Tour { TourName = "Tour 2" });
        context.SaveChanges();

        // Act
        var tours = context.Tours.ToList();

        // Assert
        Assert.Equal(2, tours.Count);
    }

    [Fact]
    public void Context_CanUpdateTour()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Original" };
        context.Tours.Add(tour);
        context.SaveChanges();

        // Act
        tour.TourName = "Updated";
        context.SaveChanges();

        // Assert
        var updatedTour = context.Tours.First();
        Assert.Equal("Updated", updatedTour.TourName);
    }

    [Fact]
    public void Context_CanDeleteTour()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Test" };
        context.Tours.Add(tour);
        context.SaveChanges();

        // Act
        context.Tours.Remove(tour);
        context.SaveChanges();

        // Assert
        Assert.Empty(context.Tours);
    }

    [Fact]
    public void Context_SupportsMultipleEntities()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Test Tour" };
        var user = new User { Email = "test@test.com" };

        // Act
        context.Tours.Add(tour);
        context.Users.Add(user);
        context.SaveChanges();

        // Assert
        Assert.Single(context.Tours);
        Assert.Single(context.Users);
    }

    [Fact]
    public void Context_CanTrackChanges()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Test" };

        // Act
        context.Tours.Add(tour);

        // Assert
        Assert.Equal(EntityState.Added, context.Entry(tour).State);
    }

    [Fact]
    public void Context_CanDetachEntities()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { TourName = "Test" };
        context.Tours.Add(tour);
        context.SaveChanges();

        // Act
        context.Entry(tour).State = EntityState.Detached;

        // Assert
        Assert.Equal(EntityState.Detached, context.Entry(tour).State);
    }

    [Fact]
    public void OnModelCreating_AppliesConfigurations()
    {
        // Arrange & Act
        var context = CreateInMemoryContext();
        var model = context.Model;

        // Assert
        Assert.NotNull(model.FindEntityType(typeof(Tour)));
        Assert.NotNull(model.FindEntityType(typeof(User)));
        Assert.NotNull(model.FindEntityType(typeof(Booking)));
    }

    [Fact]
    public void Context_SaveChanges_ReturnsSavedCount()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { TourName = "Tour 1" });
        context.Tours.Add(new Tour { TourName = "Tour 2" });

        // Act
        var savedCount = context.SaveChanges();

        // Assert
        Assert.Equal(2, savedCount);
    }

    [Fact]
    public void Context_CanHandleEmptySaveChanges()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act
        var savedCount = context.SaveChanges();

        // Assert
        Assert.Equal(0, savedCount);
    }
}
