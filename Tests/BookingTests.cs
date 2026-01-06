using System;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(default(DateTime), booking.TravelDate);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0m, booking.TotalAmount);
        Assert.Equal("Pending", booking.Status);
        Assert.Null(booking.Notes);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Id_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Id = 1;

        // Assert
        Assert.Equal(1, booking.Id);
    }

    [Fact]
    public void UserId_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = 100;

        // Assert
        Assert.Equal(100, booking.UserId);
    }

    [Fact]
    public void TourId_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 200;

        // Assert
        Assert.Equal(200, booking.TourId);
    }

    [Fact]
    public void BookingDate_DefaultsToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var booking = new Booking();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(booking.BookingDate, beforeCreation, afterCreation);
    }

    [Fact]
    public void TravelDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var travelDate = new DateTime(2026, 6, 15);

        // Act
        booking.TravelDate = travelDate;

        // Assert
        Assert.Equal(travelDate, booking.TravelDate);
    }

    [Fact]
    public void NumberOfPeople_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 4;

        // Assert
        Assert.Equal(4, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalAmount_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 1500.75m;

        // Assert
        Assert.Equal(1500.75m, booking.TotalAmount);
    }

    [Fact]
    public void Status_DefaultsToPending()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Status_CanBeSetToConfirmed()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public void Status_CanBeSetToCancelled()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Cancelled";

        // Assert
        Assert.Equal("Cancelled", booking.Status);
    }

    [Fact]
    public void Notes_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = "Special requirements";

        // Assert
        Assert.Equal("Special requirements", booking.Notes);
    }

    [Fact]
    public void Notes_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = null;

        // Assert
        Assert.Null(booking.Notes);
    }

    [Fact]
    public void CreatedDate_DefaultsToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var booking = new Booking();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(booking.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public void ModifiedDate_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var modifiedDate = DateTime.UtcNow;

        // Act
        booking.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, booking.ModifiedDate);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("System", booking.CreatedBy);
    }

    [Fact]
    public void CreatedBy_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", booking.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = "Editor";

        // Assert
        Assert.Equal("Editor", booking.ModifiedBy);
    }

    [Fact]
    public void User_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 1, Email = "test@example.com" };

        // Act
        booking.User = user;

        // Assert
        Assert.Equal(user, booking.User);
        Assert.Equal(1, booking.User.Id);
    }

    [Fact]
    public void Tour_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Test Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
    }

    [Fact]
    public void AllProperties_CanBeSetSimultaneously()
    {
        // Arrange
        var booking = new Booking();
        var now = DateTime.UtcNow;
        var travelDate = new DateTime(2026, 12, 25);
        var user = new User { Id = 50 };
        var tour = new Tour { Id = 75 };

        // Act
        booking.Id = 999;
        booking.UserId = 50;
        booking.TourId = 75;
        booking.BookingDate = now;
        booking.TravelDate = travelDate;
        booking.NumberOfPeople = 5;
        booking.TotalAmount = 2500.00m;
        booking.Status = "Confirmed";
        booking.Notes = "VIP booking";
        booking.CreatedDate = now;
        booking.ModifiedDate = now;
        booking.IsActive = true;
        booking.CreatedBy = "TestUser";
        booking.ModifiedBy = "TestModifier";
        booking.User = user;
        booking.Tour = tour;

        // Assert
        Assert.Equal(999, booking.Id);
        Assert.Equal(50, booking.UserId);
        Assert.Equal(75, booking.TourId);
        Assert.Equal(now, booking.BookingDate);
        Assert.Equal(travelDate, booking.TravelDate);
        Assert.Equal(5, booking.NumberOfPeople);
        Assert.Equal(2500.00m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.Equal("VIP booking", booking.Notes);
        Assert.Equal(now, booking.CreatedDate);
        Assert.Equal(now, booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("TestUser", booking.CreatedBy);
        Assert.Equal("TestModifier", booking.ModifiedBy);
        Assert.Equal(user, booking.User);
        Assert.Equal(tour, booking.Tour);
    }

    [Fact]
    public void NumberOfPeople_CanBeZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalAmount_CanBeZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 0m;

        // Assert
        Assert.Equal(0m, booking.TotalAmount);
    }
}
