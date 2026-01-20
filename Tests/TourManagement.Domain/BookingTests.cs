using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_SetsDefaultValues()
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
        Assert.Null(booking.Notes);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_Id_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Id = 50;

        // Assert
        Assert.Equal(50, booking.Id);
    }

    [Fact]
    public void Booking_UserId_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = 100;

        // Assert
        Assert.Equal(100, booking.UserId);
    }

    [Fact]
    public void Booking_TourId_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 25;

        // Assert
        Assert.Equal(25, booking.TourId);
    }

    [Fact]
    public void Booking_BookingDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var date = new DateTime(2024, 7, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        booking.BookingDate = date;

        // Assert
        Assert.Equal(date, booking.BookingDate);
    }

    [Fact]
    public void Booking_NumberOfPeople_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 4;

        // Assert
        Assert.Equal(4, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_TotalAmount_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 2500.75m;

        // Assert
        Assert.Equal(2500.75m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_Status_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public void Booking_Notes_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = "Special request for window seats";

        // Assert
        Assert.Equal("Special request for window seats", booking.Notes);
    }

    [Fact]
    public void Booking_CreatedDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        booking.CreatedDate = date;

        // Assert
        Assert.Equal(date, booking.CreatedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var date = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        booking.ModifiedDate = date;

        // Assert
        Assert.Equal(date, booking.ModifiedDate);
    }

    [Fact]
    public void Booking_IsActive_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_CreatedBy_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", booking.CreatedBy);
    }

    [Fact]
    public void Booking_ModifiedBy_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = "User123";

        // Assert
        Assert.Equal("User123", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_User_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var user = new UserInfo { Id = 1, Email = "test@example.com" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(1, booking.User.Id);
        Assert.Equal("test@example.com", booking.User.Email);
    }

    [Fact]
    public void Booking_Tour_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 10, TourName = "Paris Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(10, booking.Tour.Id);
        Assert.Equal("Paris Tour", booking.Tour.TourName);
    }

    [Fact]
    public void Booking_AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var booking = new Booking
        {
            Id = 999,
            UserId = 500,
            TourId = 250,
            BookingDate = new DateTime(2024, 8, 20, 14, 0, 0, DateTimeKind.Utc),
            NumberOfPeople = 6,
            TotalAmount = 5000m,
            Status = "Confirmed",
            Notes = "Group booking",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "TestUser",
            ModifiedBy = "TestAdmin"
        };

        // Assert
        Assert.Equal(999, booking.Id);
        Assert.Equal(500, booking.UserId);
        Assert.Equal(250, booking.TourId);
        Assert.Equal(6, booking.NumberOfPeople);
        Assert.Equal(5000m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal("Group booking", booking.Notes);
        Assert.True(booking.IsActive);
        Assert.Equal("TestUser", booking.CreatedBy);
        Assert.Equal("TestAdmin", booking.ModifiedBy);
    }
}
