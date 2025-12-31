using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

/// <summary>
/// Unit tests for Booking entity
/// </summary>
public class BookingTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal(0, booking.Id);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.Equal("Pending", booking.Status);
        Assert.Equal("System", booking.CreatedBy);
        Assert.False(booking.IsActive);
        Assert.Null(booking.UserId);
        Assert.Null(booking.TourId);
        Assert.Null(booking.User);
        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
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
    public void TourName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourName = "Beach Vacation";

        // Act
        booking.TourName = expectedTourName;

        // Assert
        Assert.Equal(expectedTourName, booking.TourName);
    }

    [Fact]
    public void TourName_SetToEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.TourName);
    }

    [Fact]
    public void Place_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedPlace = "Hawaii";

        // Act
        booking.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, booking.Place);
    }

    [Fact]
    public void Email_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedEmail = "customer@example.com";

        // Act
        booking.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, booking.Email);
    }

    [Fact]
    public void FirstName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedFirstName = "Jane";

        // Act
        booking.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, booking.FirstName);
    }

    [Fact]
    public void BookingDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        booking.BookingDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.BookingDate);
    }

    [Fact]
    public void Status_DefaultValue_IsPending()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.Equal("Pending", booking.Status);
    }

    [Fact]
    public void Status_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedStatus = "Confirmed";

        // Act
        booking.Status = expectedStatus;

        // Assert
        Assert.Equal(expectedStatus, booking.Status);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Cancelled")]
    [InlineData("Completed")]
    public void Status_SetToVariousValues_ReturnsCorrectValue(string status)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Status = status;

        // Assert
        Assert.Equal(status, booking.Status);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = DateTime.UtcNow;

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ReturnsCorrectValue()
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
    public void ModifiedDate_SetToNull_ReturnsNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToTrue_ReturnsTrue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = true;

        // Assert
        Assert.True(booking.IsActive);
    }

    [Fact]
    public void IsActive_SetToFalse_ReturnsFalse()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.IsActive = false;

        // Assert
        Assert.False(booking.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ReturnsCorrectValue()
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
    public void ModifiedBy_SetAndGet_ReturnsCorrectValue()
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
    public void UserId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedUserId = 100;

        // Act
        booking.UserId = expectedUserId;

        // Assert
        Assert.Equal(expectedUserId, booking.UserId);
    }

    [Fact]
    public void UserId_SetToNull_ReturnsNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.UserId = null;

        // Assert
        Assert.Null(booking.UserId);
    }

    [Fact]
    public void TourId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 200;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void TourId_SetToNull_ReturnsNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = null;

        // Assert
        Assert.Null(booking.TourId);
    }

    [Fact]
    public void User_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = new User { Id = 1, Email = "user@example.com" };

        // Act
        booking.User = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.User);
    }

    [Fact]
    public void User_SetToNull_ReturnsNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.User = null;

        // Assert
        Assert.Null(booking.User);
    }

    [Fact]
    public void Tour_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedTour = new Tour { Id = 1, TourName = "Test Tour" };

        // Act
        booking.Tour = expectedTour;

        // Assert
        Assert.Equal(expectedTour, booking.Tour);
    }

    [Fact]
    public void Tour_SetToNull_ReturnsNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Tour = null;

        // Assert
        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Booking_WithAllPropertiesSet_MaintainsValues()
    {
        // Arrange & Act
        var booking = new Booking
        {
            Id = 1,
            TourName = "European Tour",
            Place = "Paris",
            Email = "traveler@example.com",
            FirstName = "Alice",
            BookingDate = new DateTime(2024, 7, 1),
            Status = "Confirmed",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "Admin",
            UserId = 10,
            TourId = 5
        };

        // Assert
        Assert.Equal(1, booking.Id);
        Assert.Equal("European Tour", booking.TourName);
        Assert.Equal("Paris", booking.Place);
        Assert.Equal("traveler@example.com", booking.Email);
        Assert.Equal("Alice", booking.FirstName);
        Assert.Equal(new DateTime(2024, 7, 1), booking.BookingDate);
        Assert.Equal("Confirmed", booking.Status);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Equal("Admin", booking.ModifiedBy);
        Assert.Equal(10, booking.UserId);
        Assert.Equal(5, booking.TourId);
        Assert.NotNull(booking.CreatedDate);
        Assert.NotNull(booking.ModifiedDate);
    }

    [Fact]
    public void Booking_WithUserAndTourNavigation_MaintainsReferences()
    {
        // Arrange
        var user = new User { Id = 1, Email = "user@example.com", FirstName = "John", LastName = "Doe" };
        var tour = new Tour { Id = 1, TourName = "Mountain Trek", Place = "Colorado", Price = 500m };

        // Act
        var booking = new Booking
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            TourId = tour.Id,
            Tour = tour,
            TourName = tour.TourName,
            Place = tour.Place,
            Email = user.Email,
            FirstName = user.FirstName,
            BookingDate = DateTime.UtcNow,
            Status = "Confirmed"
        };

        // Assert
        Assert.NotNull(booking.User);
        Assert.NotNull(booking.Tour);
        Assert.Equal(user.Id, booking.UserId);
        Assert.Equal(tour.Id, booking.TourId);
        Assert.Equal(user.Email, booking.Email);
        Assert.Equal(tour.TourName, booking.TourName);
    }
}
