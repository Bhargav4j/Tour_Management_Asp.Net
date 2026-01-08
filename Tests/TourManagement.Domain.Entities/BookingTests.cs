using Xunit;
using System;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Equal(default(DateTime), booking.BookingDate);
        Assert.Equal(default(DateTime), booking.CreatedDate);
        Assert.Null(booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.Tour);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_Id_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 1;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Fact]
    public void Booking_TourId_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 100;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void Booking_UserId_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedUserId = 50;

        // Act
        booking.UserId = expectedUserId;

        // Assert
        Assert.Equal(expectedUserId, booking.UserId);
    }

    [Fact]
    public void Booking_TourName_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedName = "Paris Tour";

        // Act
        booking.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, booking.TourName);
    }

    [Fact]
    public void Booking_Place_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedPlace = "Paris";

        // Act
        booking.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, booking.Place);
    }

    [Fact]
    public void Booking_Email_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedEmail = "test@example.com";

        // Act
        booking.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, booking.Email);
    }

    [Fact]
    public void Booking_FirstName_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedFirstName = "John";

        // Act
        booking.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, booking.FirstName);
    }

    [Fact]
    public void Booking_BookingDate_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.Now;

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Fact]
    public void Booking_CreatedDate_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.Now;

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.Now;

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_Null()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_IsActive_SetAndGet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_IsActive_DefaultValue()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_CreatedBy_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedCreator = "Admin";

        // Act
        booking.CreatedBy = expectedCreator;

        // Assert
        Assert.Equal(expectedCreator, booking.CreatedBy);
    }

    [Fact]
    public void Booking_ModifiedBy_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedModifier = "Admin";

        // Act
        booking.ModifiedBy = expectedModifier;

        // Assert
        Assert.Equal(expectedModifier, booking.ModifiedBy);
    }

    [Fact]
    public void Booking_ModifiedBy_Null()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = null;

        // Assert
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Tour_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
    }

    [Fact]
    public void Booking_Tour_Null()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Tour = null;

        // Assert
        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Booking_User_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 1, Email = "test@example.com" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(user, booking.User);
        Assert.Equal(1, booking.User.Id);
    }

    [Fact]
    public void Booking_User_Null()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.User = null;

        // Assert
        Assert.Null(booking.User);
    }

    [Fact]
    public void Booking_AllProperties_SetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 1;
        var expectedTourId = 100;
        var expectedUserId = 50;
        var expectedTourName = "European Tour";
        var expectedPlace = "Europe";
        var expectedEmail = "john.doe@example.com";
        var expectedFirstName = "John";
        var expectedBookingDate = DateTime.Now;
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "System";
        var expectedModifiedBy = "Admin";
        var tour = new Tour { Id = 100 };
        var user = new User { Id = 50 };

        // Act
        booking.Id = expectedId;
        booking.TourId = expectedTourId;
        booking.UserId = expectedUserId;
        booking.TourName = expectedTourName;
        booking.Place = expectedPlace;
        booking.Email = expectedEmail;
        booking.FirstName = expectedFirstName;
        booking.BookingDate = expectedBookingDate;
        booking.CreatedDate = expectedCreatedDate;
        booking.ModifiedDate = expectedModifiedDate;
        booking.IsActive = expectedIsActive;
        booking.CreatedBy = expectedCreatedBy;
        booking.ModifiedBy = expectedModifiedBy;
        booking.Tour = tour;
        booking.User = user;

        // Assert
        Assert.Equal(expectedId, booking.Id);
        Assert.Equal(expectedTourId, booking.TourId);
        Assert.Equal(expectedUserId, booking.UserId);
        Assert.Equal(expectedTourName, booking.TourName);
        Assert.Equal(expectedPlace, booking.Place);
        Assert.Equal(expectedEmail, booking.Email);
        Assert.Equal(expectedFirstName, booking.FirstName);
        Assert.Equal(expectedBookingDate, booking.BookingDate);
        Assert.Equal(expectedCreatedDate, booking.CreatedDate);
        Assert.Equal(expectedModifiedDate, booking.ModifiedDate);
        Assert.Equal(expectedIsActive, booking.IsActive);
        Assert.Equal(expectedCreatedBy, booking.CreatedBy);
        Assert.Equal(expectedModifiedBy, booking.ModifiedBy);
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
        Assert.NotNull(booking.User);
        Assert.Equal(user, booking.User);
    }
}
