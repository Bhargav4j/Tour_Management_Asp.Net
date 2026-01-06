using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_InitializesDefaultValues()
    {
        // Arrange & Act
        var tour = new Tour();

        // Assert
        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.Name);
        Assert.Equal(string.Empty, tour.Place);
        Assert.Equal(0, tour.Days);
        Assert.Equal(0, tour.Price);
        Assert.Equal(string.Empty, tour.Locations);
        Assert.Null(tour.TourInfo);
        Assert.Null(tour.PicturePath);
        Assert.Equal("System", tour.CreatedBy);
        Assert.Null(tour.ModifiedBy);
        Assert.False(tour.IsActive);
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_SetProperties_ValuesAreSetCorrectly()
    {
        // Arrange
        var tour = new Tour();
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        tour.Id = 1;
        tour.Name = "Beach Paradise";
        tour.Place = "Bali";
        tour.Days = 7;
        tour.Price = 1500.50m;
        tour.Locations = "Ubud, Seminyak, Nusa Dua";
        tour.TourInfo = "Amazing beach vacation";
        tour.PicturePath = "beach.jpg";
        tour.CreatedDate = createdDate;
        tour.ModifiedDate = modifiedDate;
        tour.IsActive = true;
        tour.CreatedBy = "Admin";
        tour.ModifiedBy = "User1";

        // Assert
        Assert.Equal(1, tour.Id);
        Assert.Equal("Beach Paradise", tour.Name);
        Assert.Equal("Bali", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Ubud, Seminyak, Nusa Dua", tour.Locations);
        Assert.Equal("Amazing beach vacation", tour.TourInfo);
        Assert.Equal("beach.jpg", tour.PicturePath);
        Assert.Equal(createdDate, tour.CreatedDate);
        Assert.Equal(modifiedDate, tour.ModifiedDate);
        Assert.True(tour.IsActive);
        Assert.Equal("Admin", tour.CreatedBy);
        Assert.Equal("User1", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Name_CanBeSetAndRetrieved()
    {
        var tour = new Tour();
        var expectedName = "Mountain Adventure";
        tour.Name = expectedName;
        Assert.Equal(expectedName, tour.Name);
    }

    [Fact]
    public void Tour_Bookings_InitializesToEmptyCollection()
    {
        var tour = new Tour();
        Assert.NotNull(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }

    [Fact]
    public void Tour_Bookings_CanAddBooking()
    {
        var tour = new Tour { Id = 1, Name = "Test Tour" };
        var booking = new Booking { Id = 1, TourId = 1, Email = "test@test.com" };
        tour.Bookings.Add(booking);
        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }
}
