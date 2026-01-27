using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class BookingTests
{
    [Fact]
    public void Constructor_DefaultValues_ShouldBeInitialized()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0m, booking.TotalAmount);
        Assert.Equal("Pending", booking.Status);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Id_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 50;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Fact]
    public void UserId_SetValue_ShouldReturnValue()
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
    public void TourId_SetValue_ShouldReturnValue()
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
    public void BookingDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2026, 6, 15);

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Fact]
    public void NumberOfPeople_SetPositiveValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedPeople = 4;

        // Act
        booking.NumberOfPeople = expectedPeople;

        // Assert
        Assert.Equal(expectedPeople, booking.NumberOfPeople);
    }

    [Fact]
    public void NumberOfPeople_SetZero_ShouldReturnZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }

    [Fact]
    public void TotalAmount_SetPositiveValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedAmount = 5000.75m;

        // Act
        booking.TotalAmount = expectedAmount;

        // Assert
        Assert.Equal(expectedAmount, booking.TotalAmount);
    }

    [Fact]
    public void TotalAmount_SetZero_ShouldReturnZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 0m;

        // Assert
        Assert.Equal(0m, booking.TotalAmount);
    }

    [Fact]
    public void Status_SetValue_ShouldReturnValue()
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
    public void Status_SetDifferentStatuses_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();

        // Act & Assert
        booking.Status = "Pending";
        Assert.Equal("Pending", booking.Status);

        booking.Status = "Confirmed";
        Assert.Equal("Confirmed", booking.Status);

        booking.Status = "Cancelled";
        Assert.Equal("Cancelled", booking.Status);

        booking.Status = "Completed";
        Assert.Equal("Completed", booking.Status);
    }

    [Fact]
    public void CreatedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_SetNull_ShouldReturnNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetTrue_ShouldReturnTrue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void IsActive_SetFalse_ShouldReturnFalse()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void CreatedBy_SetValue_ShouldReturnValue()
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
    public void ModifiedBy_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedModifiedBy = "Manager";

        // Act
        booking.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, booking.ModifiedBy);
    }

    [Fact]
    public void ModifiedBy_SetNull_ShouldReturnNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = null;

        // Assert
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void User_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = new User { Id = 1, Email = "test@example.com" };

        // Act
        booking.User = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.User);
        Assert.Equal(1, booking.User.Id);
        Assert.Equal("test@example.com", booking.User.Email);
    }

    [Fact]
    public void Tour_SetValue_ShouldReturnValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedTour = new Tour { Id = 1, TourName = "Paris Tour" };

        // Act
        booking.Tour = expectedTour;

        // Assert
        Assert.Equal(expectedTour, booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
        Assert.Equal("Paris Tour", booking.Tour.TourName);
    }

    [Fact]
    public void Booking_SetAllProperties_ShouldReturnAllValues()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 1;
        var expectedUserId = 10;
        var expectedTourId = 5;
        var expectedBookingDate = new DateTime(2026, 7, 15);
        var expectedPeople = 3;
        var expectedAmount = 4500.00m;
        var expectedStatus = "Confirmed";
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "TestUser";
        var expectedModifiedBy = "TestAdmin";
        var expectedUser = new User { Id = 10 };
        var expectedTour = new Tour { Id = 5 };

        // Act
        booking.Id = expectedId;
        booking.UserId = expectedUserId;
        booking.TourId = expectedTourId;
        booking.BookingDate = expectedBookingDate;
        booking.NumberOfPeople = expectedPeople;
        booking.TotalAmount = expectedAmount;
        booking.Status = expectedStatus;
        booking.CreatedDate = expectedCreatedDate;
        booking.ModifiedDate = expectedModifiedDate;
        booking.IsActive = expectedIsActive;
        booking.CreatedBy = expectedCreatedBy;
        booking.ModifiedBy = expectedModifiedBy;
        booking.User = expectedUser;
        booking.Tour = expectedTour;

        // Assert
        Assert.Equal(expectedId, booking.Id);
        Assert.Equal(expectedUserId, booking.UserId);
        Assert.Equal(expectedTourId, booking.TourId);
        Assert.Equal(expectedBookingDate, booking.BookingDate);
        Assert.Equal(expectedPeople, booking.NumberOfPeople);
        Assert.Equal(expectedAmount, booking.TotalAmount);
        Assert.Equal(expectedStatus, booking.Status);
        Assert.Equal(expectedCreatedDate, booking.CreatedDate);
        Assert.Equal(expectedModifiedDate, booking.ModifiedDate);
        Assert.Equal(expectedIsActive, booking.IsActive);
        Assert.Equal(expectedCreatedBy, booking.CreatedBy);
        Assert.Equal(expectedModifiedBy, booking.ModifiedBy);
        Assert.Equal(expectedUser, booking.User);
        Assert.Equal(expectedTour, booking.Tour);
    }

    [Fact]
    public void TotalAmount_SetNegativeValue_ShouldAcceptValue()
    {
        // Arrange
        var booking = new Booking();
        var negativeAmount = -100m;

        // Act
        booking.TotalAmount = negativeAmount;

        // Assert
        Assert.Equal(negativeAmount, booking.TotalAmount);
    }

    [Fact]
    public void NumberOfPeople_SetNegativeValue_ShouldAcceptValue()
    {
        // Arrange
        var booking = new Booking();
        var negativeValue = -5;

        // Act
        booking.NumberOfPeople = negativeValue;

        // Assert
        Assert.Equal(negativeValue, booking.NumberOfPeople);
    }
}
