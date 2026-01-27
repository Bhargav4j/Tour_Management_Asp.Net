using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class TourTests
{
    [Fact]
    public void Constructor_DefaultValues_ShouldBeInitialized()
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
        Assert.Null(tour.PictureFileName);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Id_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 42;

        // Act
        tour.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.Id);
    }

    [Fact]
    public void TourName_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedName = "Paris City Tour";

        // Act
        tour.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, tour.TourName);
    }

    [Fact]
    public void TourName_SetEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
    }

    [Fact]
    public void Place_SetValue_ShouldReturnValue()
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
    public void Days_SetPositiveValue_ShouldReturnValue()
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
    public void Days_SetZero_ShouldReturnZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Price_SetPositiveValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 1500.50m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Price_SetZero_ShouldReturnZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0m;

        // Assert
        Assert.Equal(0m, tour.Price);
    }

    [Fact]
    public void Locations_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Eiffel Tower, Louvre, Arc de Triomphe";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void TourInfo_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "Experience the best of Paris";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void PictureFileName_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedFileName = "paris.jpg";

        // Act
        tour.PictureFileName = expectedFileName;

        // Assert
        Assert.Equal(expectedFileName, tour.PictureFileName);
    }

    [Fact]
    public void PictureFileName_SetNull_ShouldReturnNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = null;

        // Assert
        Assert.Null(tour.PictureFileName);
    }

    [Fact]
    public void CreatedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_SetNull_ShouldReturnNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetTrue_ShouldReturnTrue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void IsActive_SetFalse_ShouldReturnFalse()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void CreatedBy_SetValue_ShouldReturnValue()
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
    public void ModifiedBy_SetValue_ShouldReturnValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedModifiedBy = "Manager";

        // Act
        tour.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, tour.ModifiedBy);
    }

    [Fact]
    public void ModifiedBy_SetNull_ShouldReturnNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void Bookings_AddBooking_ShouldContainBooking()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { Id = 1, TourId = 1 };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Bookings_AddMultipleBookings_ShouldContainAllBookings()
    {
        // Arrange
        var tour = new Tour();
        var booking1 = new Booking { Id = 1, TourId = 1 };
        var booking2 = new Booking { Id = 2, TourId = 1 };

        // Act
        tour.Bookings.Add(booking1);
        tour.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(booking1, tour.Bookings);
        Assert.Contains(booking2, tour.Bookings);
    }

    [Fact]
    public void Tour_SetAllProperties_ShouldReturnAllValues()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 1;
        var expectedName = "European Adventure";
        var expectedPlace = "Europe";
        var expectedDays = 14;
        var expectedPrice = 2500.00m;
        var expectedLocations = "Paris, Rome, Berlin";
        var expectedInfo = "Best European tour";
        var expectedFileName = "europe.jpg";
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "TestUser";
        var expectedModifiedBy = "TestAdmin";

        // Act
        tour.Id = expectedId;
        tour.TourName = expectedName;
        tour.Place = expectedPlace;
        tour.Days = expectedDays;
        tour.Price = expectedPrice;
        tour.Locations = expectedLocations;
        tour.TourInfo = expectedInfo;
        tour.PictureFileName = expectedFileName;
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
        Assert.Equal(expectedFileName, tour.PictureFileName);
        Assert.Equal(expectedCreatedDate, tour.CreatedDate);
        Assert.Equal(expectedModifiedDate, tour.ModifiedDate);
        Assert.Equal(expectedIsActive, tour.IsActive);
        Assert.Equal(expectedCreatedBy, tour.CreatedBy);
        Assert.Equal(expectedModifiedBy, tour.ModifiedBy);
    }
}
