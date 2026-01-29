using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain.Entities;

public class TourTests
{
    [Fact]
    public void Tour_Constructor_InitializesWithDefaultValues()
    {
        var tour = new Tour();

        Assert.Equal(0, tour.Id);
        Assert.Equal(string.Empty, tour.Name);
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
    public void Tour_SetProperties_ShouldStoreValues()
    {
        var tour = new Tour
        {
            Id = 1,
            Name = "Paris Adventure",
            Place = "Paris",
            Days = 7,
            Price = 1500.50m,
            Locations = "Eiffel Tower, Louvre",
            TourInfo = "Amazing tour of Paris",
            PicturePath = "/images/paris.jpg",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "admin@test.com"
        };

        Assert.Equal(1, tour.Id);
        Assert.Equal("Paris Adventure", tour.Name);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(7, tour.Days);
        Assert.Equal(1500.50m, tour.Price);
        Assert.Equal("Eiffel Tower, Louvre", tour.Locations);
        Assert.Equal("Amazing tour of Paris", tour.TourInfo);
        Assert.Equal("/images/paris.jpg", tour.PicturePath);
        Assert.True(tour.IsActive);
        Assert.Equal("admin@test.com", tour.CreatedBy);
    }

    [Fact]
    public void Tour_ModifiedDate_CanBeSetAndRetrieved()
    {
        var tour = new Tour();
        var modifiedDate = DateTime.UtcNow;

        tour.ModifiedDate = modifiedDate;

        Assert.Equal(modifiedDate, tour.ModifiedDate);
    }

    [Fact]
    public void Tour_ModifiedBy_CanBeSetAndRetrieved()
    {
        var tour = new Tour();

        tour.ModifiedBy = "user@test.com";

        Assert.Equal("user@test.com", tour.ModifiedBy);
    }

    [Fact]
    public void Tour_Bookings_CanAddItems()
    {
        var tour = new Tour();
        var booking = new Booking { Id = 1, TourId = tour.Id };

        tour.Bookings.Add(booking);

        Assert.Single(tour.Bookings);
        Assert.Contains(booking, tour.Bookings);
    }

    [Fact]
    public void Tour_Price_AcceptsDecimalValues()
    {
        var tour = new Tour { Price = 999.99m };

        Assert.Equal(999.99m, tour.Price);
    }

    [Fact]
    public void Tour_Price_AcceptsZeroValue()
    {
        var tour = new Tour { Price = 0m };

        Assert.Equal(0m, tour.Price);
    }

    [Fact]
    public void Tour_Days_AcceptsPositiveInteger()
    {
        var tour = new Tour { Days = 14 };

        Assert.Equal(14, tour.Days);
    }

    [Fact]
    public void Tour_IsActive_DefaultsToFalse()
    {
        var tour = new Tour();

        Assert.False(tour.IsActive);
    }

    [Fact]
    public void Tour_IsActive_CanBeSetToTrue()
    {
        var tour = new Tour { IsActive = true };

        Assert.True(tour.IsActive);
    }

    [Fact]
    public void Tour_PicturePath_CanBeNull()
    {
        var tour = new Tour { PicturePath = null };

        Assert.Null(tour.PicturePath);
    }

    [Fact]
    public void Tour_CreatedDate_CanBeSet()
    {
        var createdDate = new DateTime(2024, 1, 1);
        var tour = new Tour { CreatedDate = createdDate };

        Assert.Equal(createdDate, tour.CreatedDate);
    }

    [Fact]
    public void Tour_Bookings_InitializesAsEmptyList()
    {
        var tour = new Tour();

        Assert.NotNull(tour.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(tour.Bookings);
        Assert.Empty(tour.Bookings);
    }
}
