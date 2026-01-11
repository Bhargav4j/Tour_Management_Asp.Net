using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using TourManagement.Web.Models;

namespace TourManagement.Web.Models.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(0, tour.TourId);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.Pic);
    }

    [Fact]
    public void Tour_TourId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 123;

        // Act
        tour.TourId = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.TourId);
    }

    [Fact]
    public void Tour_TourName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedName = "Europe Tour";

        // Act
        tour.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, tour.TourName);
    }

    [Fact]
    public void Tour_Place_SetAndGet_ReturnsCorrectValue()
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
    public void Tour_Days_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        short expectedDays = 7;

        // Act
        tour.Days = expectedDays;

        // Assert
        Assert.Equal(expectedDays, tour.Days);
    }

    [Fact]
    public void Tour_Price_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 1299.99m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Tour_Locations_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Paris, London, Rome";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void Tour_TourInfo_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "Amazing European adventure";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void Tour_Pic_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedPic = "tour-image.jpg";

        // Act
        tour.Pic = expectedPic;

        // Assert
        Assert.Equal(expectedPic, tour.Pic);
    }

    [Fact]
    public void Tour_Pic_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Pic = null;

        // Assert
        Assert.Null(tour.Pic);
    }

    [Fact]
    public void Tour_SetAllProperties_ReturnsExpectedValues()
    {
        // Arrange
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Asia Tour",
            Place = "Tokyo",
            Days = 10,
            Price = 2500.00m,
            Locations = "Tokyo, Kyoto, Osaka",
            TourInfo = "Explore the beauty of Japan",
            Pic = "japan-tour.jpg"
        };

        // Act & Assert
        Assert.Equal(1, tour.TourId);
        Assert.Equal("Asia Tour", tour.TourName);
        Assert.Equal("Tokyo", tour.Place);
        Assert.Equal(10, tour.Days);
        Assert.Equal(2500.00m, tour.Price);
        Assert.Equal("Tokyo, Kyoto, Osaka", tour.Locations);
        Assert.Equal("Explore the beauty of Japan", tour.TourInfo);
        Assert.Equal("japan-tour.jpg", tour.Pic);
    }

    [Theory]
    [InlineData("", "Paris", 7, 1000)]
    [InlineData("Europe Tour", "", 7, 1000)]
    public void Tour_ValidationAttributes_EnforceRequiredFields(
        string tourName, string place, short days, decimal price)
    {
        // Arrange
        var tour = new Tour
        {
            TourName = tourName,
            Place = place,
            Days = days,
            Price = price,
            Locations = "Some location",
            TourInfo = "Some info"
        };

        var context = new ValidationContext(tour);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(tour, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void Tour_ValidObject_PassesValidation()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "Europe Tour",
            Place = "Paris",
            Days = 7,
            Price = 1500.00m,
            Locations = "Paris, Berlin",
            TourInfo = "Wonderful European tour"
        };

        var context = new ValidationContext(tour);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(tour, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Tour_MaxLength_TourName_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = new string('a', 21),
            Place = "Paris",
            Days = 7,
            Price = 1500.00m,
            Locations = "Paris",
            TourInfo = "Info"
        };

        var context = new ValidationContext(tour);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(tour, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Tour_Days_BoundaryValue_Minimum()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = short.MinValue;

        // Assert
        Assert.Equal(short.MinValue, tour.Days);
    }

    [Fact]
    public void Tour_Days_BoundaryValue_Maximum()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = short.MaxValue;

        // Assert
        Assert.Equal(short.MaxValue, tour.Days);
    }

    [Fact]
    public void Tour_Price_NegativeValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100.00m;

        // Assert
        Assert.Equal(-100.00m, tour.Price);
    }

    [Fact]
    public void Tour_Price_ZeroValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0.00m;

        // Assert
        Assert.Equal(0.00m, tour.Price);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(14)]
    [InlineData(30)]
    public void Tour_Days_ValidValues(short days)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = days;

        // Assert
        Assert.Equal(days, tour.Days);
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(999.99)]
    [InlineData(5000.50)]
    public void Tour_Price_ValidValues(decimal price)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = price;

        // Assert
        Assert.Equal(price, tour.Price);
    }
}
