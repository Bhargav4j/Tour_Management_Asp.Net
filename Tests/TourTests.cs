using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
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
        Assert.Null(tour.Picture);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Id_SetAndGet_ShouldWork()
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
    public void TourName_SetAndGet_ShouldWork()
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
    public void Place_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedPlace = "Paris, France";

        // Act
        tour.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, tour.Place);
    }

    [Fact]
    public void Days_SetAndGet_ShouldWork()
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
    public void Price_SetAndGet_ShouldWork()
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
    public void Locations_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Paris, Lyon, Marseille";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void TourInfo_SetAndGet_ShouldWork()
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
    public void Picture_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedPicture = "picture.jpg";

        // Act
        tour.Picture = expectedPicture;

        // Assert
        Assert.Equal(expectedPicture, tour.Picture);
    }

    [Fact]
    public void Picture_SetNull_ShouldWork()
    {
        // Arrange
        var tour = new Tour { Picture = "picture.jpg" };

        // Act
        tour.Picture = null;

        // Assert
        Assert.Null(tour.Picture);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.UtcNow.AddDays(-1);

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.UtcNow;

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToFalse_ShouldWork()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedCreatedBy = "Admin";

        // Act
        tour.CreatedBy = expectedCreatedBy;

        // Assert
        Assert.Equal(expectedCreatedBy, tour.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var expectedModifiedBy = "Admin";

        // Act
        tour.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, tour.ModifiedBy);
    }

    [Fact]
    public void Bookings_SetAndGet_ShouldWork()
    {
        // Arrange
        var tour = new Tour();
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourId = 1 },
            new Booking { Id = 2, TourId = 1 }
        };

        // Act
        tour.Bookings = bookings;

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(tour.Bookings, b => b.Id == 1);
        Assert.Contains(tour.Bookings, b => b.Id == 2);
    }

    [Fact]
    public void Price_WithNegativeValue_ShouldSet()
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
    public void Days_WithZero_ShouldSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Days_WithNegativeValue_ShouldSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = -5;

        // Assert
        Assert.Equal(-5, tour.Days);
    }
}
