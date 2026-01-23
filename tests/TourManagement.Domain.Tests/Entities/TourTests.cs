using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests.Entities;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_ShouldInitializeProperties()
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
        Assert.Null(tour.PicturePath);
        Assert.Equal(string.Empty, tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.Null(tour.ModifiedDate);
        Assert.False(tour.IsActive);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Id = 456;

        // Assert
        Assert.Equal(456, tour.Id);
    }

    [Fact]
    public void Tour_SetTourName_ShouldUpdateTourNameProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "Paris Adventure";

        // Assert
        Assert.Equal("Paris Adventure", tour.TourName);
    }

    [Fact]
    public void Tour_SetPlace_ShouldUpdatePlaceProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Place = "Paris, France";

        // Assert
        Assert.Equal("Paris, France", tour.Place);
    }

    [Fact]
    public void Tour_SetDays_ShouldUpdateDaysProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 7;

        // Assert
        Assert.Equal(7, tour.Days);
    }

    [Fact]
    public void Tour_SetPrice_ShouldUpdatePriceProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 1500.50m;

        // Assert
        Assert.Equal(1500.50m, tour.Price);
    }

    [Fact]
    public void Tour_SetLocations_ShouldUpdateLocationsProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Locations = "Eiffel Tower, Louvre Museum, Notre Dame";

        // Assert
        Assert.Equal("Eiffel Tower, Louvre Museum, Notre Dame", tour.Locations);
    }

    [Fact]
    public void Tour_SetTourInfo_ShouldUpdateTourInfoProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourInfo = "Experience the beauty of Paris";

        // Assert
        Assert.Equal("Experience the beauty of Paris", tour.TourInfo);
    }

    [Fact]
    public void Tour_SetPicturePath_ShouldUpdatePicturePathProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PicturePath = "/images/paris.jpg";

        // Assert
        Assert.Equal("/images/paris.jpg", tour.PicturePath);
    }

    [Fact]
    public void Tour_SetPicturePathToNull_ShouldAcceptNull()
    {
        // Arrange
        var tour = new Tour { PicturePath = "/images/paris.jpg" };

        // Act
        tour.PicturePath = null;

        // Assert
        Assert.Null(tour.PicturePath);
    }

    [Fact]
    public void Tour_SetCreatedDate_ShouldUpdateCreatedDateProperty()
    {
        // Arrange
        var tour = new Tour();
        var createdDate = DateTime.UtcNow;

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
        var modifiedDate = DateTime.UtcNow;

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

        // Act
        tour.CreatedBy = "admin";

        // Assert
        Assert.Equal("admin", tour.CreatedBy);
    }

    [Fact]
    public void Tour_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = "admin";

        // Assert
        Assert.Equal("admin", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_SetBookings_ShouldUpdateBookingsCollection()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { Id = 1 };

        // Act
        tour.Bookings = new List<Booking> { booking };

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_AddBooking_ShouldAddToBookingsCollection()
    {
        // Arrange
        var tour = new Tour();
        var booking1 = new Booking { Id = 1 };
        var booking2 = new Booking { Id = 2 };

        // Act
        tour.Bookings.Add(booking1);
        tour.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(booking1, tour.Bookings);
        Assert.Contains(booking2, tour.Bookings);
    }

    [Fact]
    public void Tour_WithCompleteData_ShouldRetainAllProperties()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 1,
            TourName = "European Adventure",
            Place = "Europe",
            Days = 14,
            Price = 3500.00m,
            Locations = "Paris, Rome, Barcelona",
            TourInfo = "Explore the best of Europe",
            PicturePath = "/images/europe.jpg",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "system",
            ModifiedBy = "admin"
        };

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("European Adventure", tour.TourName);
        Assert.Equal("Europe", tour.Place);
        Assert.Equal(14, tour.Days);
        Assert.Equal(3500.00m, tour.Price);
        Assert.Equal("Paris, Rome, Barcelona", tour.Locations);
        Assert.Equal("Explore the best of Europe", tour.TourInfo);
        Assert.Equal("/images/europe.jpg", tour.PicturePath);
        Assert.NotNull(tour.CreatedDate);
        Assert.NotNull(tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("system", tour.CreatedBy);
        Assert.Equal("admin", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_SetZeroDays_ShouldAcceptZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Tour_SetNegativeDays_ShouldAcceptNegativeValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = -1;

        // Assert
        Assert.Equal(-1, tour.Days);
    }

    [Fact]
    public void Tour_SetZeroPrice_ShouldAcceptZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0m;

        // Assert
        Assert.Equal(0m, tour.Price);
    }

    [Fact]
    public void Tour_SetNegativePrice_ShouldAcceptNegativeValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100m;

        // Assert
        Assert.Equal(-100m, tour.Price);
    }

    [Fact]
    public void Tour_SetEmptyTourName_ShouldAcceptEmptyString()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "";

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
    }

    [Fact]
    public void Tour_SetNullModifiedBy_ShouldAcceptNull()
    {
        // Arrange
        var tour = new Tour { ModifiedBy = "admin" };

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void Tour_SetNullModifiedDate_ShouldAcceptNull()
    {
        // Arrange
        var tour = new Tour { ModifiedDate = DateTime.UtcNow };

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }
}
