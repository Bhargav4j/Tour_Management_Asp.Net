using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
    }

    [Fact]
    public void TourId_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourId = 1;

        // Assert
        Assert.Equal(1, tour.TourId);
    }

    [Fact]
    public void TourName_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = "European Adventure";

        // Assert
        Assert.Equal("European Adventure", tour.TourName);
    }

    [Fact]
    public void TourName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
    }

    [Fact]
    public void Place_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Place = "Paris";

        // Assert
        Assert.Equal("Paris", tour.Place);
    }

    [Fact]
    public void Place_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(string.Empty, tour.Place);
    }

    [Fact]
    public void Days_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 7;

        // Assert
        Assert.Equal(7, tour.Days);
    }

    [Fact]
    public void Price_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 1500.99m;

        // Assert
        Assert.Equal(1500.99m, tour.Price);
    }

    [Fact]
    public void Locations_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Locations = "Paris, Rome, Barcelona";

        // Assert
        Assert.Equal("Paris, Rome, Barcelona", tour.Locations);
    }

    [Fact]
    public void Locations_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(string.Empty, tour.Locations);
    }

    [Fact]
    public void TourInfo_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourInfo = "An amazing tour of Europe";

        // Assert
        Assert.Equal("An amazing tour of Europe", tour.TourInfo);
    }

    [Fact]
    public void TourInfo_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(string.Empty, tour.TourInfo);
    }

    [Fact]
    public void PictureFileName_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = "tour_image.jpg";

        // Assert
        Assert.Equal("tour_image.jpg", tour.PictureFileName);
    }

    [Fact]
    public void PictureFileName_ShouldBeNullable()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureFileName = null;

        // Assert
        Assert.Null(tour.PictureFileName);
    }

    [Fact]
    public void CreatedDate_ShouldSetAndGetValue()
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
    public void ModifiedDate_ShouldSetAndGetValue()
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
    public void ModifiedDate_ShouldBeNullable()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void IsActive_ShouldSetFalse()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Bookings_ShouldInitializeAsEmptyList()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Bookings_ShouldAddBooking()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { BookingId = 1, TourId = 1, UserId = 1 };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_ShouldSetAllProperties()
    {
        // Arrange
        var createdDate = DateTime.Now;
        var tour = new Tour
        {
            TourId = 1,
            TourName = "European Adventure",
            Place = "Paris",
            Days = 7,
            Price = 1500.99m,
            Locations = "Paris, Rome, Barcelona",
            TourInfo = "An amazing tour of Europe",
            PictureFileName = "tour.jpg",
            CreatedDate = createdDate,
            ModifiedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, tour.TourId);
        Assert.Equal("European Adventure", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.99m, tour.Price);
        Assert.Equal("Paris, Rome, Barcelona", tour.Locations);
        Assert.Equal("An amazing tour of Europe", tour.TourInfo);
        Assert.Equal("tour.jpg", tour.PictureFileName);
        Assert.Equal(createdDate, tour.CreatedDate);
        Assert.True(tour.IsActive);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(14)]
    [InlineData(30)]
    public void Days_ShouldAcceptDifferentValues(int days)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = days;

        // Assert
        Assert.Equal(days, tour.Days);
    }

    [Fact]
    public void Price_ShouldHandleZeroValue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0m;

        // Assert
        Assert.Equal(0m, tour.Price);
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(1500.50)]
    [InlineData(5000.99)]
    public void Price_ShouldAcceptDifferentValues(decimal price)
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = price;

        // Assert
        Assert.Equal(price, tour.Price);
    }
}
