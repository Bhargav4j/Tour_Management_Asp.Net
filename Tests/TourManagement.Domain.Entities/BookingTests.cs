using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.False(booking.IsActive);
        Assert.Equal(string.Empty, booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.Tour);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var booking = new Booking();
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        booking.Id = 1;
        booking.TourId = 100;
        booking.TourName = "Paris Tour";
        booking.Place = "Paris";
        booking.Email = "user@example.com";
        booking.FirstName = "John";
        booking.CreatedDate = createdDate;
        booking.ModifiedDate = modifiedDate;
        booking.IsActive = true;
        booking.CreatedBy = "user@example.com";
        booking.ModifiedBy = "admin@example.com";

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal(100, booking.TourId);
        Assert.Equal("Paris Tour", booking.TourName);
        Assert.Equal("Paris", booking.Place);
        Assert.Equal("user@example.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
        Assert.Equal(createdDate, booking.CreatedDate);
        Assert.Equal(modifiedDate, booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("user@example.com", booking.CreatedBy);
        Assert.Equal("admin@example.com", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Tour_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Booking_User_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_SetTour_ShouldAssignTourReference()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Test Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
        Assert.Equal("Test Tour", booking.Tour.TourName);
    }

    [Fact]
    public void Booking_SetUser_ShouldAssignUserReference()
    {
        // Arrange
        var booking = new Booking();
        var user = new UserInfo { Email = "test@example.com", FirstName = "John" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal("test@example.com", booking.User.Email);
        Assert.Equal("John", booking.User.FirstName);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = null;

        // Assert
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_TourId_ShouldAcceptPositiveInteger()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 999;

        // Assert
        Assert.Equal(999, booking.TourId);
    }
}
