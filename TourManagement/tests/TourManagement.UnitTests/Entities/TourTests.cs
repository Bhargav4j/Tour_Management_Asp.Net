using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

/// <summary>
/// Unit tests for Tour entity
/// </summary>
public class TourTests
{
    [Fact]
    public void Tour_Constructor_ShouldInitializeWithDefaultValues()
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
        Assert.Null(tour.ImageFileName);
        Assert.Equal(default(DateTime), tour.CreatedDate);
        Assert.Null(tour.ModifiedDate);
        Assert.False(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Id = 1;

        // Assert
        Assert.Equal(1, tour.Id);
    }

    [Fact]
    public void Tour_SetTourName_ShouldUpdateTourNameProperty()
    {
        // Arrange
        var tour = new Tour();
        var tourName = "Grand Canyon Adventure";

        // Act
        tour.TourName = tourName;

        // Assert
        Assert.Equal(tourName, tour.TourName);
    }

    [Fact]
    public void Tour_SetPlace_ShouldUpdatePlaceProperty()
    {
        // Arrange
        var tour = new Tour();
        var place = "Arizona";

        // Act
        tour.Place = place;

        // Assert
        Assert.Equal(place, tour.Place);
    }

    [Fact]
    public void Tour_SetDays_ShouldUpdateDaysProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 5;

        // Assert
        Assert.Equal(5, tour.Days);
    }

    [Fact]
    public void Tour_SetPrice_ShouldUpdatePriceProperty()
    {
        // Arrange
        var tour = new Tour();
        var price = 999.99m;

        // Act
        tour.Price = price;

        // Assert
        Assert.Equal(price, tour.Price);
    }

    [Fact]
    public void Tour_SetLocations_ShouldUpdateLocationsProperty()
    {
        // Arrange
        var tour = new Tour();
        var locations = "Phoenix, Sedona, Grand Canyon";

        // Act
        tour.Locations = locations;

        // Assert
        Assert.Equal(locations, tour.Locations);
    }

    [Fact]
    public void Tour_SetTourInfo_ShouldUpdateTourInfoProperty()
    {
        // Arrange
        var tour = new Tour();
        var tourInfo = "Amazing adventure through the Grand Canyon";

        // Act
        tour.TourInfo = tourInfo;

        // Assert
        Assert.Equal(tourInfo, tour.TourInfo);
    }

    [Fact]
    public void Tour_SetImageFileName_ShouldUpdateImageFileNameProperty()
    {
        // Arrange
        var tour = new Tour();
        var imageFileName = "grand_canyon.jpg";

        // Act
        tour.ImageFileName = imageFileName;

        // Assert
        Assert.Equal(imageFileName, tour.ImageFileName);
    }

    [Fact]
    public void Tour_SetCreatedDate_ShouldUpdateCreatedDateProperty()
    {
        // Arrange
        var tour = new Tour();
        var createdDate = DateTime.Now;

        // Act
        tour.CreatedDate = createdDate;

        // Assert
        Assert.Equal(createdDate, tour.CreatedDate);
    }

    [Fact]
    public void Tour_SetModifiedDate_ShouldUpdateModifiedDateProperty()
    {
        // Arrange
        var tour = new Tour();
        var modifiedDate = DateTime.Now;

        // Act
        tour.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_SetIsActive_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_SetCreatedBy_ShouldUpdateCreatedByProperty()
    {
        // Arrange
        var tour = new Tour();
        var createdBy = "Admin";

        // Act
        tour.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, tour.CreatedBy);
    }

    [Fact]
    public void Tour_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var tour = new Tour();
        var modifiedBy = "Admin";

        // Act
        tour.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, tour.ModifiedBy);
    }

    [Fact]
    public void Tour_AddBooking_ShouldAddBookingToCollection()
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
    public void Tour_SetNegativePrice_ShouldAllowNegativeValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100m;

        // Assert
        Assert.Equal(-100m, tour.Price);
    }

    [Fact]
    public void Tour_SetNegativeDays_ShouldAllowNegativeValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = -1;

        // Assert
        Assert.Equal(-1, tour.Days);
    }

    [Fact]
    public void Tour_SetEmptyStrings_ShouldAcceptEmptyStrings()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = string.Empty;
        tour.Place = string.Empty;
        tour.Locations = string.Empty;
        tour.TourInfo = string.Empty;

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
    }
}
