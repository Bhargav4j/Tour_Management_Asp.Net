using Xunit;
using System;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(default(DateTime), booking.BookingDate);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0m, booking.TotalAmount);
        Assert.Equal(string.Empty, booking.Status);
        Assert.Equal(default(DateTime), booking.CreatedDate);
        Assert.Null(booking.ModifiedDate);
        Assert.False(booking.IsActive);
        Assert.Equal(string.Empty, booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetId_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 789;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Fact]
    public void Booking_SetUserId_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedUserId = 10;

        // Act
        booking.UserId = expectedUserId;

        // Assert
        Assert.Equal(expectedUserId, booking.UserId);
    }

    [Fact]
    public void Booking_SetTourId_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 20;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void Booking_SetBookingDate_SetsValueCorrectly()
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
    public void Booking_SetNumberOfPeople_SetsValueCorrectly()
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
    public void Booking_SetTotalAmount_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedAmount = 2500.50m;

        // Act
        booking.TotalAmount = expectedAmount;

        // Assert
        Assert.Equal(expectedAmount, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetStatus_SetsValueCorrectly()
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
    public void Booking_SetCreatedDate_SetsValueCorrectly()
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
    public void Booking_SetModifiedDate_SetsValueCorrectly()
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
    public void Booking_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_SetCreatedBy_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = "admin";

        // Act
        booking.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.CreatedBy);
    }

    [Fact]
    public void Booking_SetModifiedBy_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = "admin";

        // Act
        booking.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetUser_SetsValueCorrectly()
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
    public void Booking_SetTour_SetsValueCorrectly()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Paris Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
    }

    [Fact]
    public void Booking_AllPropertiesSet_RetainsValues()
    {
        // Arrange
        var user = new User { Id = 1 };
        var tour = new Tour { Id = 2 };
        var booking = new Booking
        {
            Id = 1,
            UserId = 10,
            TourId = 20,
            BookingDate = DateTime.Now,
            NumberOfPeople = 3,
            TotalAmount = 3000m,
            Status = "Confirmed",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true,
            CreatedBy = "admin",
            ModifiedBy = "user",
            User = user,
            Tour = tour
        };

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(20, booking.TourId);
        Assert.NotNull(booking.BookingDate);
        Assert.Equal(3, booking.NumberOfPeople);
        Assert.Equal(3000m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.NotNull(booking.CreatedDate);
        Assert.NotNull(booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("admin", booking.CreatedBy);
        Assert.Equal("user", booking.ModifiedBy);
        Assert.NotNull(booking.User);
        Assert.NotNull(booking.Tour);
    }

    [Fact]
    public void Booking_SetNumberOfPeople_WithNegativeValue_SetsValue()
    {
        // Arrange
        var booking = new Booking();
        var negativeNumber = -5;

        // Act
        booking.NumberOfPeople = negativeNumber;

        // Assert
        Assert.Equal(negativeNumber, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_SetTotalAmount_WithNegativeValue_SetsValue()
    {
        // Arrange
        var booking = new Booking();
        var negativeAmount = -1000m;

        // Act
        booking.TotalAmount = negativeAmount;

        // Assert
        Assert.Equal(negativeAmount, booking.TotalAmount);
    }

    [Fact]
    public void Booking_Status_CanBeEmpty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.Status);
    }
}
