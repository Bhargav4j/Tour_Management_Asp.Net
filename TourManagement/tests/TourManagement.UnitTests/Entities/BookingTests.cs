using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

/// <summary>
/// Unit tests for Booking entity
/// </summary>
public class BookingTests
{
    [Fact]
    public void Booking_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(0, booking.UserId);
        Assert.Equal(default(DateTime), booking.BookingDate);
        Assert.Equal(0, booking.NumberOfPeople);
        Assert.Equal(0m, booking.TotalAmount);
        Assert.Equal("Pending", booking.Status);
        Assert.Null(booking.Notes);
        Assert.Equal(default(DateTime), booking.CreatedDate);
        Assert.Null(booking.ModifiedDate);
        Assert.False(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Id = 1;

        // Assert
        Assert.Equal(1, booking.Id);
    }

    [Fact]
    public void Booking_SetTourId_ShouldUpdateTourIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 5;

        // Assert
        Assert.Equal(5, booking.TourId);
    }

    [Fact]
    public void Booking_SetUserId_ShouldUpdateUserIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = 10;

        // Assert
        Assert.Equal(10, booking.UserId);
    }

    [Fact]
    public void Booking_SetBookingDate_ShouldUpdateBookingDateProperty()
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
    public void Booking_SetNumberOfPeople_ShouldUpdateNumberOfPeopleProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 4;

        // Assert
        Assert.Equal(4, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_SetTotalAmount_ShouldUpdateTotalAmountProperty()
    {
        // Arrange
        var booking = new Booking();
        var totalAmount = 1500.50m;

        // Act
        booking.TotalAmount = totalAmount;

        // Assert
        Assert.Equal(totalAmount, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetStatus_ShouldUpdateStatusProperty()
    {
        // Arrange
        var booking = new Booking();
        var status = "Confirmed";

        // Act
        booking.Status = status;

        // Assert
        Assert.Equal(status, booking.Status);
    }

    [Fact]
    public void Booking_SetNotes_ShouldUpdateNotesProperty()
    {
        // Arrange
        var booking = new Booking();
        var notes = "Special dietary requirements";

        // Act
        booking.Notes = notes;

        // Assert
        Assert.Equal(notes, booking.Notes);
    }

    [Fact]
    public void Booking_SetCreatedDate_ShouldUpdateCreatedDateProperty()
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
    public void Booking_SetModifiedDate_ShouldUpdateModifiedDateProperty()
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
    public void Booking_SetIsActive_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void Booking_SetCreatedBy_ShouldUpdateCreatedByProperty()
    {
        // Arrange
        var booking = new Booking();
        var createdBy = "Admin";

        // Act
        booking.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, booking.CreatedBy);
    }

    [Fact]
    public void Booking_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var booking = new Booking();
        var modifiedBy = "Admin";

        // Act
        booking.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetTour_ShouldUpdateTourProperty()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Grand Canyon" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(tour, booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
    }

    [Fact]
    public void Booking_SetUser_ShouldUpdateUserProperty()
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
    public void Booking_SetNegativeNumberOfPeople_ShouldAllowNegativeValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = -1;

        // Assert
        Assert.Equal(-1, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_SetNegativeTotalAmount_ShouldAllowNegativeValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = -100m;

        // Assert
        Assert.Equal(-100m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetNullNotes_ShouldAcceptNullValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Notes = null;

        // Assert
        Assert.Null(booking.Notes);
    }

    [Fact]
    public void Booking_SetEmptyStatus_ShouldAcceptEmptyString()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.Status);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("Completed")]
    public void Booking_SetStatus_ShouldAcceptVariousStatuses(string status)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = status;

        // Assert
        Assert.Equal(status, booking.Status);
    }
}
