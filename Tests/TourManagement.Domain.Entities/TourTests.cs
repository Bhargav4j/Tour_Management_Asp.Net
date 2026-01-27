using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.PicturePath);
        Assert.Equal(default(DateTime), tour.CreatedDate);
        Assert.Null(tour.ModifiedDate);
        Assert.False(tour.IsActive);
        Assert.Equal(string.Empty, tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
    }

    [Fact]
    public void Tour_SetId_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 123;

        // Act
        tour.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.Id);
    }

    [Fact]
    public void Tour_SetTourName_SetsValueCorrectly()
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
    public void Tour_SetPlace_SetsValueCorrectly()
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
    public void Tour_SetDays_SetsValueCorrectly()
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
    public void Tour_SetPrice_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 999.99m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Tour_SetLocations_SetsValueCorrectly()
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
    public void Tour_SetTourInfo_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "Amazing tour of France";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void Tour_SetPicturePath_SetsValueCorrectly()
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
    public void Tour_SetCreatedDate_SetsValueCorrectly()
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
    public void Tour_SetModifiedDate_SetsValueCorrectly()
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
    public void Tour_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_SetCreatedBy_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedUser = "admin";

        // Act
        tour.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, tour.CreatedBy);
    }

    [Fact]
    public void Tour_SetModifiedBy_SetsValueCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedUser = "admin";

        // Act
        tour.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_DefaultsToEmptyList()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_CanAddBookings()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { Id = 1 };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_SetPrice_WithNegativeValue_SetsValue()
    {
        // Arrange
        var tour = new Tour();
        var negativePrice = -100m;

        // Act
        tour.Price = negativePrice;

        // Assert
        Assert.Equal(negativePrice, tour.Price);
    }

    [Fact]
    public void Tour_SetDays_WithNegativeValue_SetsValue()
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
    public void Tour_AllPropertiesSet_RetainsValues()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Paris, Lyon",
            TourInfo = "Great tour",
            PicturePath = "/images/tour.jpg",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true,
            CreatedBy = "admin",
            ModifiedBy = "user"
        };

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(5, tour.Days);
        Assert.Equal(1000m, tour.Price);
        Assert.Equal("Paris, Lyon", tour.Locations);
        Assert.Equal("Great tour", tour.TourInfo);
        Assert.Equal("/images/tour.jpg", tour.PicturePath);
        Assert.NotNull(tour.CreatedDate);
        Assert.NotNull(tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("admin", tour.CreatedBy);
        Assert.Equal("user", tour.ModifiedBy);
    }
}
