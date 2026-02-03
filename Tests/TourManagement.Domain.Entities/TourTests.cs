using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_DefaultConstructor_ShouldInitializeWithDefaultValues()
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
        Assert.Null(tour.PictureFileName);
        Assert.False(tour.IsActive);
        Assert.Equal(string.Empty, tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var tour = new Tour();
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        tour.Id = 1;
        tour.TourName = "Paris Tour";
        tour.Place = "Paris";
        tour.Days = 5;
        tour.Price = 1500.50m;
        tour.Locations = "Eiffel Tower, Louvre";
        tour.TourInfo = "Amazing tour of Paris";
        tour.PictureFileName = "paris.jpg";
        tour.CreatedDate = createdDate;
        tour.ModifiedDate = modifiedDate;
        tour.IsActive = true;
        tour.CreatedBy = "Admin";
        tour.ModifiedBy = "User";

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(5, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("Amazing tour of Paris", tour.TourInfo);
        Assert.Equal("paris.jpg", tour.PictureFileName);
        Assert.Equal(createdDate, tour.CreatedDate);
        Assert.Equal(modifiedDate, tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("Admin", tour.CreatedBy);
        Assert.Equal("User", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_ShouldBeEmptyCollectionByDefault()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(tour.Bookings);
    }

    [Fact]
    public void Tour_AddBooking_ShouldIncreaseBookingsCount()
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
    public void Tour_Days_ShouldAcceptPositiveInteger()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 10;

        // Assert
        Assert.Equal(10, tour.Days);
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
    public void Tour_PictureFileName_CanBeNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = null;

        // Assert
        Assert.Null(tour.PictureFileName);
    }
}
