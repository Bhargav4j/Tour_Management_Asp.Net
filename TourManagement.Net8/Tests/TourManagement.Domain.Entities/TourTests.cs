using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Linq;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Constructor_CreatesTour_WithDefaultValues()
    {
        // Act
        var tour = new Tour();

        // Assert
        Assert.NotNull(tour);
        Assert.Equal(string.Empty, tour.TourName);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal("System", tour.CreatedBy);
        Assert.NotNull(tour.Bookings);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var tour = new Tour();

        // Act
        tour.Id = 1;
        tour.TourName = "Paris Tour";
        tour.Place = "Paris";
        tour.Days = 5;
        tour.Price = 1000;
        tour.Locations = "Eiffel Tower, Louvre";
        tour.TourInfo = "Best of Paris";
        tour.PictureFileName = "paris.jpg";
        tour.IsActive = true;

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(5, tour.Days);
        Assert.Equal(1000, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("Best of Paris", tour.TourInfo);
        Assert.Equal("paris.jpg", tour.PictureFileName);
        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Bookings_CanAddBookings()
    {
        // Arrange
        var tour = new Tour();
        var booking1 = new Booking { Id = 1, NumberOfPersons = 2 };
        var booking2 = new Booking { Id = 2, NumberOfPersons = 3 };

        // Act
        tour.Bookings.Add(booking1);
        tour.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, tour.Bookings.Count);
        Assert.Contains(booking1, tour.Bookings);
        Assert.Contains(booking2, tour.Bookings);
    }
}
