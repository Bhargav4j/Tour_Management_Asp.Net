using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

/// <summary>
/// Unit tests for Tour entity
/// </summary>
public class TourTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Description);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0m, tour.Price);
        Assert.Equal(0, tour.Duration);
        Assert.Equal(string.Empty, tour.ImageUrl);
        Assert.Equal("System", tour.CreatedBy);
        Assert.False(tour.IsActive);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedId = 456;

        // Act
        tour.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, tour.Id);
    }

    [Fact]
    public void TourName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedName = "Grand Canyon Adventure";

        // Act
        tour.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, tour.TourName);
    }

    [Fact]
    public void TourName_SetToEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
    }

    [Fact]
    public void Description_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDescription = "Amazing tour of the Grand Canyon";

        // Act
        tour.Description = expectedDescription;

        // Assert
        Assert.Equal(expectedDescription, tour.Description);
    }

    [Fact]
    public void Place_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedPlace = "Arizona";

        // Act
        tour.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, tour.Place);
    }

    [Fact]
    public void Price_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 299.99m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Price_SetToZero_ReturnsZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0m;

        // Assert
        Assert.Equal(0m, tour.Price);
    }

    [Fact]
    public void Price_SetToNegative_ReturnsNegative()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100m;

        // Assert
        Assert.Equal(-100m, tour.Price);
    }

    [Fact]
    public void Duration_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDuration = 5;

        // Act
        tour.Duration = expectedDuration;

        // Assert
        Assert.Equal(expectedDuration, tour.Duration);
    }

    [Fact]
    public void Duration_SetToZero_ReturnsZero()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Duration = 0;

        // Assert
        Assert.Equal(0, tour.Duration);
    }

    [Fact]
    public void ImageUrl_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedUrl = "https://example.com/image.jpg";

        // Act
        tour.ImageUrl = expectedUrl;

        // Assert
        Assert.Equal(expectedUrl, tour.ImageUrl);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = DateTime.UtcNow;

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ReturnsCorrectValue()
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
    public void ModifiedDate_SetToNull_ReturnsNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToTrue_ReturnsTrue()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = true;

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void IsActive_SetToFalse_ReturnsFalse()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ReturnsCorrectValue()
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
    public void ModifiedBy_SetAndGet_ReturnsCorrectValue()
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
    public void Bookings_AddBooking_ContainsBooking()
    {
        // Arrange
        var tour = new Tour();
        var booking = new Booking { Id = 1, TourName = "Test Tour" };

        // Act
        tour.Bookings.Add(booking);

        // Assert
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Bookings_AddMultipleBookings_ContainsAllBookings()
    {
        // Arrange
        var tour = new Tour();
        var booking1 = new Booking { Id = 1, TourName = "Tour 1" };
        var booking2 = new Booking { Id = 2, TourName = "Tour 2" };

        // Act
        tour.Bookings.Add(booking1);
        tour.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(booking1, tour.Bookings);
        Assert.Contains(booking2, tour.Bookings);
    }

    [Fact]
    public void Tour_WithAllPropertiesSet_MaintainsValues()
    {
        // Arrange & Act
        var tour = new Tour
        {
            Id = 1,
            TourName = "Safari Adventure",
            Description = "Exciting safari tour in Africa",
            Place = "Kenya",
            Price = 1999.99m,
            Duration = 7,
            ImageUrl = "https://example.com/safari.jpg",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "Admin"
        };

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Safari Adventure", tour.TourName);
        Assert.Equal("Exciting safari tour in Africa", tour.Description);
        Assert.Equal("Kenya", tour.Place);
        Assert.Equal(1999.99m, tour.Price);
        Assert.Equal(7, tour.Duration);
        Assert.Equal("https://example.com/safari.jpg", tour.ImageUrl);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Equal("Admin", tour.ModifiedBy);
        Assert.NotNull(tour.CreatedDate);
        Assert.NotNull(tour.ModifiedDate);
    }
}
