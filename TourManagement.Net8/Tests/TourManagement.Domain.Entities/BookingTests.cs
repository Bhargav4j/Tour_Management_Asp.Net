using Xunit;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.Domain.Entities.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_CreatesBooking_WithDefaultValues()
    {
        // Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
        Assert.Equal("Pending", booking.Status);
        Assert.Equal("System", booking.CreatedBy);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var date = DateTime.Now;

        // Act
        booking.Id = 1;
        booking.UserId = 10;
        booking.TourId = 20;
        booking.BookingDate = date;
        booking.NumberOfPersons = 3;
        booking.TotalAmount = 3000;
        booking.Status = "Confirmed";
        booking.IsActive = true;

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(20, booking.TourId);
        Assert.Equal(date, booking.BookingDate);
        Assert.Equal(3, booking.NumberOfPersons);
        Assert.Equal(3000, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void NavigationProperties_CanBeSet()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User" };
        var tour = new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris" };

        // Act
        booking.User = user;
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.User);
        Assert.NotNull(booking.Tour);
        Assert.Equal("test@test.com", booking.User.Email);
        Assert.Equal("Paris Tour", booking.Tour.TourName);
    }
}
