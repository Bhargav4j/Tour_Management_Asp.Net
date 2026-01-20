using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_SetsDefaultValues()
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
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.Null(tour.ModifiedDate);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Id_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Id = 123;

        // Assert
        Assert.Equal(123, tour.Id);
    }

    [Fact]
    public void Tour_TourName_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "Paris Tour";

        // Assert
        Assert.Equal("Paris Tour", tour.TourName);
    }

    [Fact]
    public void Tour_Place_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Place = "Paris";

        // Assert
        Assert.Equal("Paris", tour.Place);
    }

    [Fact]
    public void Tour_Days_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 7;

        // Assert
        Assert.Equal(7, tour.Days);
    }

    [Fact]
    public void Tour_Price_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 1500.50m;

        // Assert
        Assert.Equal(1500.50m, tour.Price);
    }

    [Fact]
    public void Tour_Locations_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Locations = "Paris, Lyon, Marseille";

        // Assert
        Assert.Equal("Paris, Lyon, Marseille", tour.Locations);
    }

    [Fact]
    public void Tour_TourInfo_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourInfo = "Amazing tour of France";

        // Assert
        Assert.Equal("Amazing tour of France", tour.TourInfo);
    }

    [Fact]
    public void Tour_PictureFileName_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = "paris.jpg";

        // Assert
        Assert.Equal("paris.jpg", tour.PictureFileName);
    }

    [Fact]
    public void Tour_CreatedDate_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        tour.CreatedDate = date;

        // Assert
        Assert.Equal(date, tour.CreatedDate);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var date = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        tour.ModifiedDate = date;

        // Assert
        Assert.Equal(date, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_IsActive_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_CreatedBy_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", tour.CreatedBy);
    }

    [Fact]
    public void Tour_ModifiedBy_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = "User123";

        // Assert
        Assert.Equal("User123", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var bookings = new List<Booking> { new Booking(), new Booking() };

        // Act
        tour.Bookings = bookings;

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
    }

    [Fact]
    public void Tour_AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var tour = new Tour
        {
            Id = 999,
            TourName = "Complete Tour",
            Place = "Tokyo",
            Days = 10,
            Price = 3000m,
            Locations = "Tokyo, Kyoto, Osaka",
            TourInfo = "Full Japan experience",
            PictureFileName = "japan.jpg",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestUser"
        };

        // Assert
        Assert.Equal(999, tour.Id);
        Assert.Equal("Complete Tour", tour.TourName);
        Assert.Equal("Tokyo", tour.Place);
        Assert.Equal(10, tour.Days);
        Assert.Equal(3000m, tour.Price);
        Assert.Equal("Tokyo, Kyoto, Osaka", tour.Locations);
        Assert.Equal("Full Japan experience", tour.TourInfo);
        Assert.Equal("japan.jpg", tour.PictureFileName);
        Assert.True(tour.IsActive);
        Assert.Equal("TestUser", tour.CreatedBy);
        Assert.Equal("TestUser", tour.ModifiedBy);
    }
}
