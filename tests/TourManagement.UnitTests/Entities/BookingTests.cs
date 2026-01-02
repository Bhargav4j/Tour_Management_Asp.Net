using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0, booking.TotalAmount);
        Assert.Equal("Pending", booking.Status);
        Assert.False(booking.IsActive);
        Assert.Equal(string.Empty, booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Properties_ShouldBeSettable()
    {
        // Arrange
        var booking = new Booking();
        var bookingDate = DateTime.UtcNow;
        var now = DateTime.UtcNow;

        // Act
        booking.Id = 1;
        booking.UserId = 10;
        booking.TourId = 5;
        booking.BookingDate = bookingDate;
        booking.NumberOfPeople = 4;
        booking.TotalAmount = 6000.00m;
        booking.Status = "Confirmed";
        booking.CreatedDate = now;
        booking.ModifiedDate = now;
        booking.IsActive = true;
        booking.CreatedBy = "system";
        booking.ModifiedBy = "admin";

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(5, booking.TourId);
        Assert.Equal(bookingDate, booking.BookingDate);
        Assert.Equal(4, booking.NumberOfPeople);
        Assert.Equal(6000.00m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal(now, booking.CreatedDate);
        Assert.Equal(now, booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("system", booking.CreatedBy);
        Assert.Equal("admin", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Status_DefaultShouldBePending()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Booking_Status_ShouldAcceptConfirmedValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public void Booking_Status_ShouldAcceptCancelledValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Cancelled";

        // Assert
        Assert.Equal("Cancelled", booking.Status);
    }

    [Fact]
    public void Booking_NumberOfPeople_ShouldAcceptPositiveValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 5;

        // Assert
        Assert.Equal(5, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_TotalAmount_ShouldAcceptDecimalValues()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 1234.56m;

        // Assert
        Assert.Equal(1234.56m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_User_Navigation_ShouldBeSettable()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 1, Email = "test@example.com" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(1, booking.User.Id);
    }

    [Fact]
    public void Booking_Tour_Navigation_ShouldBeSettable()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Paris Trip" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
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
    public void Booking_IsActive_ShouldToggle()
    {
        // Arrange
        var booking = new Booking { IsActive = false };

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }
}
