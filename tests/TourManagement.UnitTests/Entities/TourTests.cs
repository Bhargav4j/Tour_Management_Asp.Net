using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

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
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.Null(tour.PicturePath);
        Assert.False(tour.IsActive);
        Assert.Equal(string.Empty, tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Properties_ShouldBeSettable()
    {
        // Arrange
        var tour = new Tour();
        var now = DateTime.UtcNow;

        // Act
        tour.Id = 1;
        tour.TourName = "Paris Adventure";
        tour.Place = "Paris";
        tour.Days = 7;
        tour.Price = 1500.50m;
        tour.Locations = "Eiffel Tower, Louvre";
        tour.TourInfo = "Amazing tour of Paris";
        tour.PicturePath = "/images/paris.jpg";
        tour.CreatedDate = now;
        tour.ModifiedDate = now;
        tour.IsActive = true;
        tour.CreatedBy = "admin";
        tour.ModifiedBy = "admin";

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Adventure", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("Amazing tour of Paris", tour.TourInfo);
        Assert.Equal("/images/paris.jpg", tour.PicturePath);
        Assert.Equal(now, tour.CreatedDate);
        Assert.Equal(now, tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("admin", tour.CreatedBy);
        Assert.Equal("admin", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Price_ShouldAcceptDecimalValues()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 999.99m;

        // Assert
        Assert.Equal(999.99m, tour.Price);
    }

    [Fact]
    public void Tour_Days_ShouldAcceptPositiveIntegers()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 14;

        // Assert
        Assert.Equal(14, tour.Days);
    }

    [Fact]
    public void Tour_Days_ShouldAcceptZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Tour_Bookings_ShouldAllowAddingBookings()
    {
        // Arrange
        var tour = new Tour { Id = 1 };
        var booking = new Booking { Id = 1, TourId = 1 };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_PicturePath_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PicturePath = null;

        // Assert
        Assert.Null(tour.PicturePath);
    }

    [Fact]
    public void Tour_ModifiedBy_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void Tour_IsActive_ShouldToggle()
    {
        // Arrange
        var tour = new Tour { IsActive = false };

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }
}
