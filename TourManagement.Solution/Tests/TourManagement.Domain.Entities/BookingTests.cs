using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_InitializesWithDefaultValues()
    {
        var booking = new Booking();

        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.UserEmail);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.False(booking.IsActive);
        Assert.Equal(string.Empty, booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.Tour);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_SetProperties_ShouldStoreValues()
    {
        var booking = new Booking
        {
            Id = 1,
            TourId = 10,
            TourName = "Paris Adventure",
            Place = "Paris",
            UserEmail = "user@example.com",
            FirstName = "John",
            BookingDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "admin@test.com"
        };

        Assert.Equal(1, booking.Id);
        Assert.Equal(10, booking.TourId);
        Assert.Equal("Paris Adventure", booking.TourName);
        Assert.Equal("Paris", booking.Place);
        Assert.Equal("user@example.com", booking.UserEmail);
        Assert.Equal("John", booking.FirstName);
        Assert.True(booking.IsActive);
        Assert.Equal("admin@test.com", booking.CreatedBy);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeSetAndRetrieved()
    {
        var booking = new Booking();
        var modifiedDate = DateTime.UtcNow;

        booking.ModifiedDate = modifiedDate;

        Assert.Equal(modifiedDate, booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeNull()
    {
        var booking = new Booking { ModifiedDate = null };

        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeSetAndRetrieved()
    {
        var booking = new Booking { ModifiedBy = "modifier@test.com" };

        Assert.Equal("modifier@test.com", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeNull()
    {
        var booking = new Booking { ModifiedBy = null };

        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Tour_CanBeSetAndRetrieved()
    {
        var tour = new Tour { Id = 5, Name = "Test Tour" };
        var booking = new Booking { TourId = 5, Tour = tour };

        Assert.NotNull(booking.Tour);
        Assert.Equal(5, booking.Tour.Id);
        Assert.Equal("Test Tour", booking.Tour.Name);
    }

    [Fact]
    public void Booking_User_CanBeSetAndRetrieved()
    {
        var user = new User { Email = "user@test.com", FirstName = "Jane" };
        var booking = new Booking { UserEmail = "user@test.com", User = user };

        Assert.NotNull(booking.User);
        Assert.Equal("user@test.com", booking.User.Email);
        Assert.Equal("Jane", booking.User.FirstName);
    }

    [Fact]
    public void Booking_IsActive_DefaultsToFalse()
    {
        var booking = new Booking();

        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_IsActive_CanBeSetToTrue()
    {
        var booking = new Booking { IsActive = true };

        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_BookingDate_CanBeSet()
    {
        var bookingDate = new DateTime(2024, 6, 15);
        var booking = new Booking { BookingDate = bookingDate };

        Assert.Equal(bookingDate, booking.BookingDate);
    }

    [Fact]
    public void Booking_CreatedDate_CanBeSet()
    {
        var createdDate = DateTime.UtcNow;
        var booking = new Booking { CreatedDate = createdDate };

        Assert.Equal(createdDate, booking.CreatedDate);
    }

    [Fact]
    public void Booking_AllStringProperties_InitializeAsEmpty()
    {
        var booking = new Booking();

        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.UserEmail);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Equal(string.Empty, booking.CreatedBy);
    }

    [Fact]
    public void Booking_Tour_CanBeNull()
    {
        var booking = new Booking { Tour = null };

        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Booking_User_CanBeNull()
    {
        var booking = new Booking { User = null };

        Assert.Null(booking.User);
    }
}
