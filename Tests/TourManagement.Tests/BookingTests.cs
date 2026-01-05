using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.BookingId);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(default(DateTime), booking.BookingDate);
        Assert.Equal(default(DateTime), booking.CreatedDate);
        Assert.Null(booking.ModifiedDate);
        Assert.False(booking.IsActive);
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Booking_BookingId_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 101;

        // Act
        booking.BookingId = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.BookingId);
    }

    [Fact]
    public void Booking_TourId_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 5;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void Booking_Email_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedEmail = "booking@example.com";

        // Act
        booking.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, booking.Email);
    }

    [Fact]
    public void Booking_Email_WithEmptyString_ShouldSetCorrectly()
    {
        // Arrange
        var booking = new Booking { Email = "test@example.com" };

        // Act
        booking.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.Email);
    }

    [Fact]
    public void Booking_BookingDate_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 7, 15);

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Fact]
    public void Booking_CreatedDate_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 1, 1);

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_WithNull_ShouldSetCorrectly()
    {
        // Arrange
        var booking = new Booking { ModifiedDate = DateTime.Now };

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_IsActive_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_IsActive_WithFalse_ShouldSetCorrectly()
    {
        // Arrange
        var booking = new Booking { IsActive = true };

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_Status_ShouldDefaultToPending()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Booking_Status_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedStatus = "Confirmed";

        // Act
        booking.Status = expectedStatus;

        // Assert
        Assert.Equal(expectedStatus, booking.Status);
    }

    [Fact]
    public void Booking_Status_WithCancelled_ShouldSetCorrectly()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Cancelled";

        // Assert
        Assert.Equal("Cancelled", booking.Status);
    }

    [Fact]
    public void Booking_Status_WithCompleted_ShouldSetCorrectly()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Completed";

        // Assert
        Assert.Equal("Completed", booking.Status);
    }

    [Fact]
    public void Booking_Tour_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { TourId = 1, TourName = "Paris Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(1, booking.Tour.TourId);
    }

    [Fact]
    public void Booking_User_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Email = "test@example.com", FirstName = "John" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(user, booking.User);
        Assert.Equal("test@example.com", booking.User.Email);
    }

    [Fact]
    public void Booking_AllProperties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedBookingId = 100;
        var expectedTourId = 5;
        var expectedEmail = "user@example.com";
        var expectedBookingDate = new DateTime(2024, 8, 1);
        var expectedCreatedDate = new DateTime(2024, 7, 1);
        var expectedModifiedDate = new DateTime(2024, 7, 15);
        var expectedIsActive = true;
        var expectedStatus = "Confirmed";
        var expectedTour = new Tour { TourId = 5 };
        var expectedUser = new User { Email = "user@example.com" };

        // Act
        booking.BookingId = expectedBookingId;
        booking.TourId = expectedTourId;
        booking.Email = expectedEmail;
        booking.BookingDate = expectedBookingDate;
        booking.CreatedDate = expectedCreatedDate;
        booking.ModifiedDate = expectedModifiedDate;
        booking.IsActive = expectedIsActive;
        booking.Status = expectedStatus;
        booking.Tour = expectedTour;
        booking.User = expectedUser;

        // Assert
        Assert.Equal(expectedBookingId, booking.BookingId);
        Assert.Equal(expectedTourId, booking.TourId);
        Assert.Equal(expectedEmail, booking.Email);
        Assert.Equal(expectedBookingDate, booking.BookingDate);
        Assert.Equal(expectedCreatedDate, booking.CreatedDate);
        Assert.Equal(expectedModifiedDate, booking.ModifiedDate);
        Assert.Equal(expectedIsActive, booking.IsActive);
        Assert.Equal(expectedStatus, booking.Status);
        Assert.Equal(expectedTour, booking.Tour);
        Assert.Equal(expectedUser, booking.User);
    }

    [Fact]
    public void Booking_TourIdAndTourNavigation_ShouldBeConsistent()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { TourId = 10, TourName = "Rome Tour" };

        // Act
        booking.TourId = 10;
        booking.Tour = tour;

        // Assert
        Assert.Equal(booking.TourId, booking.Tour.TourId);
    }

    [Fact]
    public void Booking_EmailAndUserNavigation_ShouldBeConsistent()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Email = "jane@example.com", FirstName = "Jane" };

        // Act
        booking.Email = "jane@example.com";
        booking.User = user;

        // Assert
        Assert.Equal(booking.Email, booking.User.Email);
    }

    [Fact]
    public void Booking_StatusTransitions_ShouldUpdateCorrectly()
    {
        // Arrange
        var booking = new Booking { Status = "Pending" };

        // Act & Assert - Pending to Confirmed
        booking.Status = "Confirmed";
        Assert.Equal("Confirmed", booking.Status);

        // Act & Assert - Confirmed to Completed
        booking.Status = "Completed";
        Assert.Equal("Completed", booking.Status);

        // Act & Assert - Can also be Cancelled
        booking.Status = "Cancelled";
        Assert.Equal("Cancelled", booking.Status);
    }
}
