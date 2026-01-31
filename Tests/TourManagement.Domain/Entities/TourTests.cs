using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.Picture);
        Assert.True(tour.CreatedDate <= DateTime.UtcNow);
        Assert.Null(tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Id_CanSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 100;

        // Act
        tour.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.Id);
    }

    [Theory]
    [InlineData("Beach Paradise")]
    [InlineData("Mountain Adventure")]
    [InlineData("")]
    public void Tour_TourName_CanSetAndGet(string tourName)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = tourName;

        // Assert
        Assert.Equal(tourName, tour.TourName);
    }

    [Theory]
    [InlineData("Hawaii")]
    [InlineData("Switzerland")]
    [InlineData("")]
    public void Tour_Place_CanSetAndGet(string place)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Place = place;

        // Assert
        Assert.Equal(place, tour.Place);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(30)]
    [InlineData(-1)]
    public void Tour_Days_CanSetAndGet(int days)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = days;

        // Assert
        Assert.Equal(days, tour.Days);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(9999.99)]
    [InlineData(-100)]
    public void Tour_Price_CanSetAndGet(decimal price)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = price;

        // Assert
        Assert.Equal(price, tour.Price);
    }

    [Theory]
    [InlineData("Beach, Mountain")]
    [InlineData("")]
    [InlineData(null)]
    public void Tour_Locations_CanSetAndGet(string? locations)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Locations = locations ?? string.Empty;

        // Assert
        Assert.Equal(locations ?? string.Empty, tour.Locations);
    }

    [Theory]
    [InlineData("Exciting tour package")]
    [InlineData("")]
    public void Tour_TourInfo_CanSetAndGet(string tourInfo)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourInfo = tourInfo;

        // Assert
        Assert.Equal(tourInfo, tour.TourInfo);
    }

    [Theory]
    [InlineData("image.jpg")]
    [InlineData(null)]
    [InlineData("")]
    public void Tour_Picture_CanSetAndGet(string? picture)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Picture = picture;

        // Assert
        Assert.Equal(picture, tour.Picture);
    }

    [Fact]
    public void Tour_CreatedDate_CanSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2024, 1, 1);

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_CanSetAndGetNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_CanSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Tour_IsActive_CanSetAndGet(bool isActive)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = isActive;

        // Assert
        Assert.Equal(isActive, tour.IsActive);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User123")]
    [InlineData("")]
    public void Tour_CreatedBy_CanSetAndGet(string createdBy)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, tour.CreatedBy);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData(null)]
    [InlineData("")]
    public void Tour_ModifiedBy_CanSetAndGet(string? modifiedBy)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_CanSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var bookings = new List<Booking>
        {
            new Booking(),
            new Booking()
        };

        // Act
        tour.Bookings = bookings;

        // Assert
        Assert.Equal(bookings, tour.Bookings);
        Assert.Equal(2, tour.Bookings.Count);
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
    public void Tour_AllProperties_CanSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.UtcNow;

        // Act
        tour.Id = 5;
        tour.TourName = "Complete Tour";
        tour.Place = "Europe";
        tour.Days = 10;
        tour.Price = 2500.00m;
        tour.Locations = "Paris, Rome, Berlin";
        tour.TourInfo = "A comprehensive European tour";
        tour.Picture = "europe.jpg";
        tour.CreatedDate = expectedDate;
        tour.ModifiedDate = expectedDate;
        tour.IsActive = false;
        tour.CreatedBy = "TestUser";
        tour.ModifiedBy = "AdminUser";

        // Assert
        Assert.Equal(5, tour.Id);
        Assert.Equal("Complete Tour", tour.TourName);
        Assert.Equal("Europe", tour.Place);
        Assert.Equal(10, tour.Days);
        Assert.Equal(2500.00m, tour.Price);
        Assert.Equal("Paris, Rome, Berlin", tour.Locations);
        Assert.Equal("A comprehensive European tour", tour.TourInfo);
        Assert.Equal("europe.jpg", tour.Picture);
        Assert.Equal(expectedDate, tour.CreatedDate);
        Assert.Equal(expectedDate, tour.ModifiedDate);
        Assert.False(tour.IsActive);
        Assert.Equal("TestUser", tour.CreatedBy);
        Assert.Equal("AdminUser", tour.ModifiedBy);
    }
}
