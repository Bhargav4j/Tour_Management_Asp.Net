using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0m, booking.TotalAmount);
        Assert.Equal("Pending", booking.BookingStatus);
        Assert.Null(booking.Notes);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Id_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 123;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Fact]
    public void TourId_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 456;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void UserId_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedUserId = 789;

        // Act
        booking.UserId = expectedUserId;

        // Assert
        Assert.Equal(expectedUserId, booking.UserId);
    }

    [Fact]
    public void NumberOfPeople_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedNumber = 4;

        // Act
        booking.NumberOfPeople = expectedNumber;

        // Assert
        Assert.Equal(expectedNumber, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalAmount_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedAmount = 1999.99m;

        // Act
        booking.TotalAmount = expectedAmount;

        // Assert
        Assert.Equal(expectedAmount, booking.TotalAmount);
    }

    [Fact]
    public void BookingDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.UtcNow.AddDays(7);

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Fact]
    public void BookingStatus_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedStatus = "Confirmed";

        // Act
        booking.BookingStatus = expectedStatus;

        // Assert
        Assert.Equal(expectedStatus, booking.BookingStatus);
    }

    [Fact]
    public void Notes_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedNotes = "Special dietary requirements";

        // Act
        booking.Notes = expectedNotes;

        // Assert
        Assert.Equal(expectedNotes, booking.Notes);
    }

    [Fact]
    public void Notes_SetNull_ShouldWork()
    {
        // Arrange
        var booking = new Booking { Notes = "Some notes" };

        // Act
        booking.Notes = null;

        // Assert
        Assert.Null(booking.Notes);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.UtcNow.AddDays(-1);

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.UtcNow;

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToFalse_ShouldWork()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedCreatedBy = "Admin";

        // Act
        booking.CreatedBy = expectedCreatedBy;

        // Assert
        Assert.Equal(expectedCreatedBy, booking.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedModifiedBy = "Admin";

        // Act
        booking.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, booking.ModifiedBy);
    }

    [Fact]
    public void Tour_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedTour = new Tour { Id = 1, TourName = "Paris Tour" };

        // Act
        booking.Tour = expectedTour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
        Assert.Equal("Paris Tour", booking.Tour.TourName);
    }

    [Fact]
    public void User_SetAndGet_ShouldWork()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = new User { Id = 1, Username = "john_doe" };

        // Act
        booking.User = expectedUser;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(1, booking.User.Id);
        Assert.Equal("john_doe", booking.User.Username);
    }

    [Fact]
    public void NumberOfPeople_WithZero_ShouldSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }

    [Fact]
    public void NumberOfPeople_WithNegativeValue_ShouldSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = -1;

        // Assert
        Assert.Equal(-1, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalAmount_WithZero_ShouldSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 0m;

        // Assert
        Assert.Equal(0m, booking.TotalAmount);
    }

    [Fact]
    public void TotalAmount_WithNegativeValue_ShouldSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = -100m;

        // Assert
        Assert.Equal(-100m, booking.TotalAmount);
    }
}
