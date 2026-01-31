using Xunit;
using System;
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
        Assert.NotNull(booking);
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0, booking.TotalAmount);
        Assert.True(booking.BookingDate <= DateTime.UtcNow);
        Assert.Equal("Pending", booking.BookingStatus);
        Assert.Null(booking.Notes);
        Assert.True(booking.CreatedDate <= DateTime.UtcNow);
        Assert.Null(booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Id_CanSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 42;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Booking_TourId_CanSetAndGet(int tourId)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = tourId;

        // Assert
        Assert.Equal(tourId, booking.TourId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Booking_UserId_CanSetAndGet(int userId)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = userId;

        // Assert
        Assert.Equal(userId, booking.UserId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Booking_NumberOfPeople_CanSetAndGet(int numberOfPeople)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = numberOfPeople;

        // Assert
        Assert.Equal(numberOfPeople, booking.NumberOfPeople);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100.50)]
    [InlineData(9999.99)]
    [InlineData(-100)]
    public void Booking_TotalAmount_CanSetAndGet(decimal totalAmount)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = totalAmount;

        // Assert
        Assert.Equal(totalAmount, booking.TotalAmount);
    }

    [Fact]
    public void Booking_BookingDate_CanSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 12, 25);

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("")]
    public void Booking_BookingStatus_CanSetAndGet(string status)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.BookingStatus = status;

        // Assert
        Assert.Equal(status, booking.BookingStatus);
    }

    [Theory]
    [InlineData("Special request")]
    [InlineData(null)]
    [InlineData("")]
    public void Booking_Notes_CanSetAndGet(string? notes)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = notes;

        // Assert
        Assert.Equal(notes, booking.Notes);
    }

    [Fact]
    public void Booking_CreatedDate_CanSetAndGet()
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
    public void Booking_ModifiedDate_CanSetAndGetNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_ModifiedDate_CanSetAndGetValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Booking_IsActive_CanSetAndGet(bool isActive)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = isActive;

        // Assert
        Assert.Equal(isActive, booking.IsActive);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User123")]
    [InlineData("")]
    public void Booking_CreatedBy_CanSetAndGet(string createdBy)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, booking.CreatedBy);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData(null)]
    [InlineData("")]
    public void Booking_ModifiedBy_CanSetAndGet(string? modifiedBy)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, booking.ModifiedBy);
    }

    [Fact]
    public void Booking_Tour_CanSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Test Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
    }

    [Fact]
    public void Booking_User_CanSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 1, Username = "testuser" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(user, booking.User);
        Assert.Equal(1, booking.User.Id);
    }

    [Fact]
    public void Booking_AllProperties_CanSetAndGetCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedBookingDate = new DateTime(2024, 12, 1);
        var expectedCreatedDate = new DateTime(2024, 11, 1);
        var expectedModifiedDate = new DateTime(2024, 11, 15);
        var tour = new Tour { Id = 5, TourName = "European Tour" };
        var user = new User { Id = 10, Username = "johndoe" };

        // Act
        booking.Id = 100;
        booking.TourId = 5;
        booking.UserId = 10;
        booking.NumberOfPeople = 3;
        booking.TotalAmount = 3000.00m;
        booking.BookingDate = expectedBookingDate;
        booking.BookingStatus = "Confirmed";
        booking.Notes = "Window seats preferred";
        booking.CreatedDate = expectedCreatedDate;
        booking.ModifiedDate = expectedModifiedDate;
        booking.IsActive = true;
        booking.CreatedBy = "Agent";
        booking.ModifiedBy = "Manager";
        booking.Tour = tour;
        booking.User = user;

        // Assert
        Assert.Equal(100, booking.Id);
        Assert.Equal(5, booking.TourId);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(3, booking.NumberOfPeople);
        Assert.Equal(3000.00m, booking.TotalAmount);
        Assert.Equal(expectedBookingDate, booking.BookingDate);
        Assert.Equal("Confirmed", booking.BookingStatus);
        Assert.Equal("Window seats preferred", booking.Notes);
        Assert.Equal(expectedCreatedDate, booking.CreatedDate);
        Assert.Equal(expectedModifiedDate, booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("Agent", booking.CreatedBy);
        Assert.Equal("Manager", booking.ModifiedBy);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(user, booking.User);
    }
}
