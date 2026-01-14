using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for Tour entity
/// </summary>
public class TourTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Equal(string.Empty, tour.TourInfo);
        Assert.True(tour.IsActive);
        Assert.Equal("System", tour.CreatedBy);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Id_CanBeSetAndGet()
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
    public void TourName_CanBeSetAndGet()
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
    public void TourName_EmptyString_IsValid()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, tour.TourName);
    }

    [Fact]
    public void Place_CanBeSetAndGet()
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
    public void Days_PositiveValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDays = 5;

        // Act
        tour.Days = expectedDays;

        // Assert
        Assert.Equal(expectedDays, tour.Days);
    }

    [Fact]
    public void Days_ZeroValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = 0;

        // Assert
        Assert.Equal(0, tour.Days);
    }

    [Fact]
    public void Days_NegativeValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Days = -1;

        // Assert
        Assert.Equal(-1, tour.Days);
    }

    [Fact]
    public void Price_PositiveValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();
        var expectedPrice = 599.99m;

        // Act
        tour.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, tour.Price);
    }

    [Fact]
    public void Price_ZeroValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = 0;

        // Assert
        Assert.Equal(0, tour.Price);
    }

    [Fact]
    public void Price_NegativeValue_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Price = -100m;

        // Assert
        Assert.Equal(-100m, tour.Price);
    }

    [Fact]
    public void Locations_CanBeSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedLocations = "Grand Canyon, Las Vegas, Phoenix";

        // Act
        tour.Locations = expectedLocations;

        // Assert
        Assert.Equal(expectedLocations, tour.Locations);
    }

    [Fact]
    public void TourInfo_CanBeSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedInfo = "Explore the breathtaking Grand Canyon";

        // Act
        tour.TourInfo = expectedInfo;

        // Assert
        Assert.Equal(expectedInfo, tour.TourInfo);
    }

    [Fact]
    public void PictureUrl_CanBeSetToNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.PictureUrl = null;

        // Assert
        Assert.Null(tour.PictureUrl);
    }

    [Fact]
    public void PictureUrl_CanBeSetToValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedUrl = "https://example.com/tour.jpg";

        // Act
        tour.PictureUrl = expectedUrl;

        // Assert
        Assert.Equal(expectedUrl, tour.PictureUrl);
    }

    [Fact]
    public void CreatedDate_CanBeSetAndGet()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2024, 1, 15);

        // Act
        tour.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedDate = null;

        // Assert
        Assert.Null(tour.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedDate = new DateTime(2024, 2, 20);

        // Act
        tour.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, tour.ModifiedDate);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.IsActive = false;

        // Assert
        Assert.False(tour.IsActive);
    }

    [Fact]
    public void CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal("System", tour.CreatedBy);
    }

    [Fact]
    public void CreatedBy_CanBeChanged()
    {
        // Arrange
        var tour = new Tour();
        var expectedUser = "AdminUser";

        // Act
        tour.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, tour.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSetToNull()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.ModifiedBy = null;

        // Assert
        Assert.Null(tour.ModifiedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSetToValue()
    {
        // Arrange
        var tour = new Tour();
        var expectedUser = "EditorUser";

        // Act
        tour.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, tour.ModifiedBy);
    }

    [Fact]
    public void Bookings_InitializesAsEmptyList()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Bookings_CanAddBooking()
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
    public void Bookings_CanAddMultipleBookings()
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
    public void Bookings_CanBeReplacedWithNewCollection()
    {
        // Arrange
        var tour = new Tour();
        var newBookings = new List<Booking> { new Booking { Id = 1 }, new Booking { Id = 2 } };

        // Act
        tour.Bookings = newBookings;

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Equal(newBookings, tour.Bookings);
    }

    [Fact]
    public void Tour_AllPropertiesSet_MaintainsValues()
    {
        // Arrange
        var tour = new Tour
        {
            Id = 100,
            TourName = "European Adventure",
            Place = "Europe",
            Days = 14,
            Price = 2500.00m,
            Locations = "Paris, Rome, London",
            TourInfo = "Comprehensive European tour",
            PictureUrl = "https://example.com/europe.jpg",
            CreatedDate = new DateTime(2024, 1, 1),
            ModifiedDate = new DateTime(2024, 1, 15),
            IsActive = true,
            CreatedBy = "Admin",
            ModifiedBy = "Editor"
        };

        // Act & Assert
        Assert.Equal(100, tour.Id);
        Assert.Equal("European Adventure", tour.TourName);
        Assert.Equal("Europe", tour.Place);
        Assert.Equal(14, tour.Days);
        Assert.Equal(2500.00m, tour.Price);
        Assert.Equal("Paris, Rome, London", tour.Locations);
        Assert.Equal("Comprehensive European tour", tour.TourInfo);
        Assert.Equal("https://example.com/europe.jpg", tour.PictureUrl);
        Assert.Equal(new DateTime(2024, 1, 1), tour.CreatedDate);
        Assert.Equal(new DateTime(2024, 1, 15), tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("Admin", tour.CreatedBy);
        Assert.Equal("Editor", tour.ModifiedBy);
    }
}
