using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
    }

    [Fact]
    public void BookingId_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.BookingId = 1;

        // Assert
        Assert.Equal(1, booking.BookingId);
    }

    [Fact]
    public void UserId_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = 10;

        // Assert
        Assert.Equal(10, booking.UserId);
    }

    [Fact]
    public void TourId_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 20;

        // Assert
        Assert.Equal(20, booking.TourId);
    }

    [Fact]
    public void BookingDate_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var bookingDate = DateTime.Now;

        // Act
        booking.BookingDate = bookingDate;

        // Assert
        Assert.Equal(bookingDate, booking.BookingDate);
    }

    [Fact]
    public void NumberOfPeople_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 5;

        // Assert
        Assert.Equal(5, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalPrice_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalPrice = 500.50m;

        // Assert
        Assert.Equal(500.50m, booking.TotalPrice);
    }

    [Fact]
    public void Status_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Status_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public void CreatedDate_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var createdDate = DateTime.Now;

        // Act
        booking.CreatedDate = createdDate;

        // Assert
        Assert.Equal(createdDate, booking.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var modifiedDate = DateTime.Now;

        // Act
        booking.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, booking.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_ShouldBeNullable()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void IsActive_ShouldSetFalse()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void User_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { UserId = 1, Email = "test@example.com" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(1, booking.User.UserId);
    }

    [Fact]
    public void Tour_ShouldSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { TourId = 1, TourName = "Test Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.TourId);
    }

    [Fact]
    public void Booking_ShouldSetAllProperties()
    {
        // Arrange
        var bookingDate = DateTime.Now;
        var createdDate = DateTime.Now;
        var booking = new Booking
        {
            BookingId = 1,
            UserId = 10,
            TourId = 20,
            BookingDate = bookingDate,
            NumberOfPeople = 5,
            TotalPrice = 500.50m,
            Status = "Confirmed",
            CreatedDate = createdDate,
            ModifiedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, booking.BookingId);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(20, booking.TourId);
        Assert.Equal(bookingDate, booking.BookingDate);
        Assert.Equal(5, booking.NumberOfPeople);
        Assert.Equal(500.50m, booking.TotalPrice);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal(createdDate, booking.CreatedDate);
        Assert.True(booking.IsActive);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("Completed")]
    public void Status_ShouldAcceptDifferentStatusValues(string status)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = status;

        // Assert
        Assert.Equal(status, booking.Status);
    }

    [Fact]
    public void TotalPrice_ShouldHandleZeroValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalPrice = 0m;

        // Assert
        Assert.Equal(0m, booking.TotalPrice);
    }

    [Fact]
    public void NumberOfPeople_ShouldHandleZeroValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }
}
