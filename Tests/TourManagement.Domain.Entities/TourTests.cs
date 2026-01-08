using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_SetsDefaultValues()
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
    }

    [Fact]
    public void Tour_Id_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 100;

        // Act
        tour.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.Id);
    }

    [Fact]
    public void Tour_TourName_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedName = "Paris Adventure";

        // Act
        tour.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, tour.TourName);
    }

    [Fact]
    public void Tour_Place_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedPlace = "Paris";

        // Act
        tour.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, tour.Place);
    }

    [Fact]
    public void Tour_Days_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDays = 7;

        // Act
        tour.Days = expectedDays;

        // Assert
        Assert.Equal(expectedDays, tour.Days);
    }

    [Fact]
    public void Tour_Days_NegativeValue()
    {
        // Arrange
        var tour = new Tour();
        var negativeDays = -5;

        // Act
        tour.Days = negativeDays;

        // Assert
        Assert.Equal(negativeDays, tour.Days);
    }

    [Fact]
    public void Tour_Price_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 1999.99m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Tour_Price_ZeroValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0m;

        // Assert
        Assert.Equal(0m, tour.Price);
    }

    [Fact]
    public void Tour_Price_NegativeValue()
    {
        // Arrange
        var tour = new Tour();
        var negativePrice = -100.00m;

        // Act
        tour.Price = negativePrice;

        // Assert
        Assert.Equal(negativePrice, tour.Price);
    }

    [Fact]
    public void Tour_Locations_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Paris, Lyon, Nice";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void Tour_TourInfo_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "Amazing tour through France";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void Tour_PicturePath_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedPath = "/images/paris.jpg";

        // Act
        tour.PicturePath = expectedPath;

        // Assert
        Assert.Equal(expectedPath, tour.PicturePath);
    }

    [Fact]
    public void Tour_PicturePath_Null()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PicturePath = null;

        // Assert
        Assert.Null(tour.PicturePath);
    }

    [Fact]
    public void Tour_CreatedDate_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.Now;

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.Now;

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_Null()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void Tour_IsActive_SetAndGet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_IsActive_DefaultValue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_CreatedBy_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedCreator = "Admin";

        // Act
        tour.CreatedBy = expectedCreator;

        // Assert
        Assert.Equal(expectedCreator, tour.CreatedBy);
    }

    [Fact]
    public void Tour_ModifiedBy_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedModifier = "Admin";

        // Act
        tour.ModifiedBy = expectedModifier;

        // Assert
        Assert.Equal(expectedModifier, tour.ModifiedBy);
    }

    [Fact]
    public void Tour_ModifiedBy_Null()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_InitializedAsEmptyList()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_CanAddBooking()
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
    public void Tour_AllProperties_SetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 1;
        var expectedName = "European Tour";
        var expectedPlace = "Europe";
        var expectedDays = 10;
        var expectedPrice = 2500.00m;
        var expectedLocations = "Paris, Rome, Berlin";
        var expectedInfo = "Complete Europe Tour";
        var expectedPath = "/images/europe.jpg";
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "Admin";
        var expectedModifiedBy = "Manager";

        // Act
        tour.Id = expectedId;
        tour.TourName = expectedName;
        tour.Place = expectedPlace;
        tour.Days = expectedDays;
        tour.Price = expectedPrice;
        tour.Locations = expectedLocations;
        tour.TourInfo = expectedInfo;
        tour.PicturePath = expectedPath;
        tour.CreatedDate = expectedCreatedDate;
        tour.ModifiedDate = expectedModifiedDate;
        tour.IsActive = expectedIsActive;
        tour.CreatedBy = expectedCreatedBy;
        tour.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedId, tour.Id);
        Assert.Equal(expectedName, tour.TourName);
        Assert.Equal(expectedPlace, tour.Place);
        Assert.Equal(expectedDays, tour.Days);
        Assert.Equal(expectedPrice, tour.Price);
        Assert.Equal(expectedLocations, tour.Locations);
        Assert.Equal(expectedInfo, tour.TourInfo);
        Assert.Equal(expectedPath, tour.PicturePath);
        Assert.Equal(expectedCreatedDate, tour.CreatedDate);
        Assert.Equal(expectedModifiedDate, tour.ModifiedDate);
        Assert.Equal(expectedIsActive, tour.IsActive);
        Assert.Equal(expectedCreatedBy, tour.CreatedBy);
        Assert.Equal(expectedModifiedBy, tour.ModifiedBy);
    }
}
