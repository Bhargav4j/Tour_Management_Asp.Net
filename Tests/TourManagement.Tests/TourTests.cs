using Xunit;
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
        Assert.Equal(0, tour.TourId);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.ImageFileName);
        Assert.False(tour.IsActive);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_TourId_ShouldSetAndGetCorrectly()
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
    public void Tour_TourName_ShouldSetAndGetCorrectly()
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
    public void Tour_Place_ShouldSetAndGetCorrectly()
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
    public void Tour_Days_ShouldSetAndGetCorrectly()
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
    public void Tour_Days_WithZero_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Tour_Days_WithNegative_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = -5;

        // Assert
        Assert.Equal(-5, tour.Days);
    }

    [Fact]
    public void Tour_Price_ShouldSetAndGetCorrectly()
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
    public void Tour_Price_WithZero_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0;

        // Assert
        Assert.Equal(0, tour.Price);
    }

    [Fact]
    public void Tour_Price_WithNegative_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100.50m;

        // Assert
        Assert.Equal(-100.50m, tour.Price);
    }

    [Fact]
    public void Tour_Locations_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Eiffel Tower, Louvre, Notre Dame";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void Tour_TourInfo_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "A wonderful journey through Paris";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void Tour_ImageFileName_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedFileName = "paris-tour.jpg";

        // Act
        tour.ImageFileName = expectedFileName;

        // Assert
        Assert.Equal(expectedFileName, tour.ImageFileName);
    }

    [Fact]
    public void Tour_ImageFileName_WithNull_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour { ImageFileName = "test.jpg" };

        // Act
        tour.ImageFileName = null;

        // Assert
        Assert.Null(tour.ImageFileName);
    }

    [Fact]
    public void Tour_CreatedDate_ShouldSetAndGetCorrectly()
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
    public void Tour_ModifiedDate_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_WithNull_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour { ModifiedDate = DateTime.Now };

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void Tour_IsActive_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_IsActive_WithFalse_ShouldSetCorrectly()
    {
        // Arrange
        var tour = new Tour { IsActive = true };

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_Bookings_ShouldInitializeAsEmptyCollection()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_ShouldAddBookingCorrectly()
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
    public void Tour_Bookings_ShouldAddMultipleBookingsCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var booking1 = new Booking();
        var booking2 = new Booking();

        // Act
        tour.Bookings.Add(booking1);
        tour.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(booking1, tour.Bookings);
        Assert.Contains(booking2, tour.Bookings);
    }

    [Fact]
    public void Tour_AllProperties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 1;
        var expectedName = "Paris Tour";
        var expectedPlace = "Paris";
        var expectedDays = 5;
        var expectedPrice = 1500.00m;
        var expectedLocations = "Eiffel Tower, Louvre";
        var expectedInfo = "Great tour";
        var expectedImageFileName = "paris.jpg";
        var expectedCreatedDate = new DateTime(2024, 1, 1);
        var expectedModifiedDate = new DateTime(2024, 1, 2);
        var expectedIsActive = true;

        // Act
        tour.TourId = expectedId;
        tour.TourName = expectedName;
        tour.Place = expectedPlace;
        tour.Days = expectedDays;
        tour.Price = expectedPrice;
        tour.Locations = expectedLocations;
        tour.TourInfo = expectedInfo;
        tour.ImageFileName = expectedImageFileName;
        tour.CreatedDate = expectedCreatedDate;
        tour.ModifiedDate = expectedModifiedDate;
        tour.IsActive = expectedIsActive;

        // Assert
        Assert.Equal(expectedId, tour.TourId);
        Assert.Equal(expectedName, tour.TourName);
        Assert.Equal(expectedPlace, tour.Place);
        Assert.Equal(expectedDays, tour.Days);
        Assert.Equal(expectedPrice, tour.Price);
        Assert.Equal(expectedLocations, tour.Locations);
        Assert.Equal(expectedInfo, tour.TourInfo);
        Assert.Equal(expectedImageFileName, tour.ImageFileName);
        Assert.Equal(expectedCreatedDate, tour.CreatedDate);
        Assert.Equal(expectedModifiedDate, tour.ModifiedDate);
        Assert.Equal(expectedIsActive, tour.IsActive);
    }
}
