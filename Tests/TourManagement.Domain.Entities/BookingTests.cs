using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_InitializesDefaultValues()
    {
        var booking = new Booking();
        Assert.Equal(0, booking.Id);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.TourId);
        Assert.Null(booking.UserId);
    }

    [Fact]
    public void Booking_SetProperties_ValuesAreSetCorrectly()
    {
        var booking = new Booking();
        var bookingDate = DateTime.UtcNow;
        booking.Id = 1;
        booking.TourName = "Beach Paradise";
        booking.Place = "Bali";
        booking.Email = "john@example.com";
        booking.FirstName = "John";
        booking.BookingDate = bookingDate;
        booking.IsActive = true;
        booking.TourId = 10;
        booking.UserId = 20;

        Assert.Equal(1, booking.Id);
        Assert.Equal("Beach Paradise", booking.TourName);
        Assert.Equal("Bali", booking.Place);
        Assert.Equal("john@example.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
        Assert.True(booking.IsActive);
        Assert.Equal(10, booking.TourId);
        Assert.Equal(20, booking.UserId);
    }

    [Fact]
    public void Booking_Tour_CanBeSet()
    {
        var booking = new Booking();
        var tour = new Tour { Id = 1, Name = "Test Tour" };
        booking.Tour = tour;
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
    }

    [Fact]
    public void Booking_User_CanBeSet()
    {
        var booking = new Booking();
        var user = new User { Id = 1, Email = "test@test.com" };
        booking.User = user;
        Assert.NotNull(booking.User);
        Assert.Equal(user, booking.User);
    }
}
