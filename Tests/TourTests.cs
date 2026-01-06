using System;
using System.Collections.Generic;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.PicturePath);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Id_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Id = 1;

        // Assert
        Assert.Equal(1, tour.Id);
    }

    [Fact]
    public void TourName_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "Paris Tour";

        // Assert
        Assert.Equal("Paris Tour", tour.TourName);
    }

    [Fact]
    public void Place_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Place = "Paris";

        // Assert
        Assert.Equal("Paris", tour.Place);
    }

    [Fact]
    public void Days_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 5;

        // Assert
        Assert.Equal(5, tour.Days);
    }

    [Fact]
    public void Price_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 999.99m;

        // Assert
        Assert.Equal(999.99m, tour.Price);
    }

    [Fact]
    public void Locations_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Locations = "Paris, London, Rome";

        // Assert
        Assert.Equal("Paris, London, Rome", tour.Locations);
    }

    [Fact]
    public void TourInfo_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourInfo = "Amazing tour of Europe";

        // Assert
        Assert.Equal("Amazing tour of Europe", tour.TourInfo);
    }

    [Fact]
    public void PicturePath_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PicturePath = "/images/tour.jpg";

        // Assert
        Assert.Equal("/images/tour.jpg", tour.PicturePath);
    }

    [Fact]
    public void PicturePath_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PicturePath = null;

        // Assert
        Assert.Null(tour.PicturePath);
    }

    [Fact]
    public void CreatedDate_DefaultsToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var tour = new Tour();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(tour.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public void ModifiedDate_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var modifiedDate = DateTime.UtcNow;

        // Act
        tour.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal("System", tour.CreatedBy);
    }

    [Fact]
    public void CreatedBy_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", tour.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = "Editor";

        // Assert
        Assert.Equal("Editor", tour.ModifiedBy);
    }

    [Fact]
    public void Bookings_InitializesToEmptyList()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Bookings_CanAddBooking()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking();

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void AllProperties_CanBeSetSimultaneously()
    {
        // Arrange
        var tour = new Tour();
        var now = DateTime.UtcNow;

        // Act
        tour.Id = 100;
        tour.TourName = "Full Tour";
        tour.Place = "Europe";
        tour.Days = 10;
        tour.Price = 1500.50m;
        tour.Locations = "Multiple";
        tour.TourInfo = "Complete tour package";
        tour.PicturePath = "/path/to/image.jpg";
        tour.CreatedDate = now;
        tour.ModifiedDate = now;
        tour.IsActive = false;
        tour.CreatedBy = "TestUser";
        tour.ModifiedBy = "TestModifier";

        // Assert
        Assert.Equal(100, tour.Id);
        Assert.Equal("Full Tour", tour.TourName);
        Assert.Equal("Europe", tour.Place);
        Assert.Equal(10, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Multiple", tour.Locations);
        Assert.Equal("Complete tour package", tour.TourInfo);
        Assert.Equal("/path/to/image.jpg", tour.PicturePath);
        Assert.Equal(now, tour.CreatedDate);
        Assert.Equal(now, tour.ModifiedDate);
        Assert.False(tour.IsActive);
        Assert.Equal("TestUser", tour.CreatedBy);
        Assert.Equal("TestModifier", tour.ModifiedBy);
    }
}
