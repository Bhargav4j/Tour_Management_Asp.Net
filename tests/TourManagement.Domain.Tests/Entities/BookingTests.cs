using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_ShouldInitializeProperties()
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
        Assert.Equal(0, booking.TotalAmount);
        Assert.Equal(string.Empty, booking.Status);
        Assert.Equal(string.Empty, booking.CreatedBy);
        Assert.Null(booking.ModifiedBy);
        Assert.Null(booking.ModifiedDate);
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void Booking_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Id = 789;

        // Assert
        Assert.Equal(789, booking.Id);
    }

    [Fact]
    public void Booking_SetUserId_ShouldUpdateUserIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = 123;

        // Assert
        Assert.Equal(123, booking.UserId);
    }

    [Fact]
    public void Booking_SetTourId_ShouldUpdateTourIdProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 456;

        // Assert
        Assert.Equal(456, booking.TourId);
    }

    [Fact]
    public void Booking_SetBookingDate_ShouldUpdateBookingDateProperty()
    {
        // Arrange
        var booking = new Booking();
        var bookingDate = new DateTime(2024, 6, 15);

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

        // Act
        booking.TotalAmount = 2500.75m;

        // Assert
        Assert.Equal(2500.75m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetStatus_ShouldUpdateStatusProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "Confirmed";

        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public void Booking_SetCreatedDate_ShouldUpdateCreatedDateProperty()
    {
        // Arrange
        var booking = new Booking();
        var createdDate = DateTime.UtcNow;

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
        var modifiedDate = DateTime.UtcNow;

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

        // Act
        booking.CreatedBy = "system";

        // Assert
        Assert.Equal("system", booking.CreatedBy);
    }

    [Fact]
    public void Booking_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = "admin";

        // Assert
        Assert.Equal("admin", booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetUser_ShouldUpdateUserNavigationProperty()
    {
        // Arrange
        var booking = new Booking();
        var user = new User { Id = 123 };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal(123, booking.User.Id);
    }

    [Fact]
    public void Booking_SetTour_ShouldUpdateTourNavigationProperty()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 456 };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(456, booking.Tour.Id);
    }

    [Fact]
    public void Booking_WithCompleteData_ShouldRetainAllProperties()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "John", LastName = "Doe" };
        var tour = new Tour { Id = 2, TourName = "Paris Tour" };
        var booking = new Booking
        {
            Id = 10,
            UserId = 1,
            TourId = 2,
            BookingDate = new DateTime(2024, 6, 15),
            NumberOfPeople = 3,
            TotalAmount = 4500.00m,
            Status = "Confirmed",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "system",
            ModifiedBy = "admin",
            User = user,
            Tour = tour
        };

        // Assert
        Assert.Equal(10, booking.Id);
        Assert.Equal(1, booking.UserId);
        Assert.Equal(2, booking.TourId);
        Assert.Equal(new DateTime(2024, 6, 15), booking.BookingDate);
        Assert.Equal(3, booking.NumberOfPeople);
        Assert.Equal(4500.00m, booking.TotalAmount);
        Assert.Equal("Confirmed", booking.Status);
        Assert.NotNull(booking.CreatedDate);
        Assert.NotNull(booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("system", booking.CreatedBy);
        Assert.Equal("admin", booking.ModifiedBy);
        Assert.NotNull(booking.User);
        Assert.NotNull(booking.Tour);
    }

    [Fact]
    public void Booking_SetZeroNumberOfPeople_ShouldAcceptZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = 0;

        // Assert
        Assert.Equal(0, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_SetNegativeNumberOfPeople_ShouldAcceptNegativeValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.NumberOfPeople = -1;

        // Assert
        Assert.Equal(-1, booking.NumberOfPeople);
    }

    [Fact]
    public void Booking_SetZeroTotalAmount_ShouldAcceptZero()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = 0m;

        // Assert
        Assert.Equal(0m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetNegativeTotalAmount_ShouldAcceptNegativeValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TotalAmount = -500m;

        // Assert
        Assert.Equal(-500m, booking.TotalAmount);
    }

    [Fact]
    public void Booking_SetEmptyStatus_ShouldAcceptEmptyString()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = "";

        // Assert
        Assert.Equal(string.Empty, booking.Status);
    }

    [Fact]
    public void Booking_SetNullModifiedBy_ShouldAcceptNull()
    {
        // Arrange
        var booking = new Booking { ModifiedBy = "admin" };

        // Act
        booking.ModifiedBy = null;

        // Assert
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void Booking_SetNullModifiedDate_ShouldAcceptNull()
    {
        // Arrange
        var booking = new Booking { ModifiedDate = DateTime.UtcNow };

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_SetDifferentStatuses_ShouldUpdateStatusCorrectly()
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
    }
}
