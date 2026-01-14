using Xunit;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for Booking entity
/// </summary>
public class BookingTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(0, booking.Id);
        Assert.Equal(0, booking.TourId);
        Assert.Equal(string.Empty, booking.TourName);
        Assert.Equal(string.Empty, booking.Place);
        Assert.Equal(string.Empty, booking.Email);
        Assert.Equal(string.Empty, booking.FirstName);
        Assert.True(booking.IsActive);
        Assert.Equal("System", booking.CreatedBy);
        Assert.Null(booking.Tour);
        Assert.Null(booking.User);
    }

    [Fact]
    public void Id_CanBeSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 42;

        // Act
        booking.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.Id);
    }

    [Fact]
    public void TourId_CanBeSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourId = 123;

        // Act
        booking.TourId = expectedTourId;

        // Assert
        Assert.Equal(expectedTourId, booking.TourId);
    }

    [Fact]
    public void TourId_ZeroValue_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = 0;

        // Assert
        Assert.Equal(0, booking.TourId);
    }

    [Fact]
    public void TourId_NegativeValue_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = -1;

        // Assert
        Assert.Equal(-1, booking.TourId);
    }

    [Fact]
    public void TourName_CanBeSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedName = "Grand Canyon Adventure";

        // Act
        booking.TourName = expectedName;

        // Assert
        Assert.Equal(expectedName, booking.TourName);
    }

    [Fact]
    public void TourName_EmptyString_IsValid()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.TourName);
    }

    [Fact]
    public void Place_CanBeSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedPlace = "Arizona";

        // Act
        booking.Place = expectedPlace;

        // Assert
        Assert.Equal(expectedPlace, booking.Place);
    }

    [Fact]
    public void Email_CanBeSetAndGet()
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
    public void Email_EmptyString_IsValid()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, booking.Email);
    }

    [Fact]
    public void FirstName_CanBeSetAndGet()
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
    public void BookingDate_CanBeSetAndGet()
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
    public void BookingDate_MinValue_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.BookingDate = DateTime.MinValue;

        // Assert
        Assert.Equal(DateTime.MinValue, booking.BookingDate);
    }

    [Fact]
    public void BookingDate_MaxValue_CanBeSet()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.BookingDate = DateTime.MaxValue;

        // Assert
        Assert.Equal(DateTime.MaxValue, booking.BookingDate);
    }

    [Fact]
    public void CreatedDate_CanBeSetAndGet()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 1, 15);

        // Act
        booking.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedDate = null;

        // Assert
        Assert.Null(booking.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedDate = new DateTime(2024, 2, 20);

        // Act
        booking.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, booking.ModifiedDate);
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
    public void CreatedBy_CanBeChanged()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = "AdminUser";

        // Act
        booking.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSetToNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.ModifiedBy = null;

        // Assert
        Assert.Null(booking.ModifiedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSetToValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedUser = "EditorUser";

        // Act
        booking.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, booking.ModifiedBy);
    }

    [Fact]
    public void Tour_CanBeSetToNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Tour = null;

        // Assert
        Assert.Null(booking.Tour);
    }

    [Fact]
    public void Tour_CanBeSetToValue()
    {
        // Arrange
        var booking = new Booking();
        var tour = new Tour { Id = 1, TourName = "Sample Tour" };

        // Act
        booking.Tour = tour;

        // Assert
        Assert.NotNull(booking.Tour);
        Assert.Equal(1, booking.Tour.Id);
        Assert.Equal("Sample Tour", booking.Tour.TourName);
    }

    [Fact]
    public void User_CanBeSetToNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.User = null;

        // Assert
        Assert.Null(booking.User);
    }

    [Fact]
    public void User_CanBeSetToValue()
    {
        // Arrange
        var booking = new Booking();
        var user = new UserInfo { Email = "test@example.com", FirstName = "John" };

        // Act
        booking.User = user;

        // Assert
        Assert.NotNull(booking.User);
        Assert.Equal("test@example.com", booking.User.Email);
        Assert.Equal("John", booking.User.FirstName);
    }

    [Fact]
    public void Booking_AllPropertiesSet_MaintainsValues()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Sample Tour" };
        var user = new UserInfo { Email = "test@example.com", FirstName = "John" };
        var booking = new Booking
        {
            Id = 100,
            TourId = 1,
            TourName = "Sample Tour",
            Place = "Arizona",
            Email = "test@example.com",
            FirstName = "John",
            BookingDate = new DateTime(2024, 6, 15),
            CreatedDate = new DateTime(2024, 1, 1),
            ModifiedDate = new DateTime(2024, 1, 15),
            IsActive = true,
            CreatedBy = "Admin",
            ModifiedBy = "Editor",
            Tour = tour,
            User = user
        };

        // Act & Assert
        Assert.Equal(100, booking.Id);
        Assert.Equal(1, booking.TourId);
        Assert.Equal("Sample Tour", booking.TourName);
        Assert.Equal("Arizona", booking.Place);
        Assert.Equal("test@example.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
        Assert.Equal(new DateTime(2024, 6, 15), booking.BookingDate);
        Assert.Equal(new DateTime(2024, 1, 1), booking.CreatedDate);
        Assert.Equal(new DateTime(2024, 1, 15), booking.ModifiedDate);
        Assert.True(booking.IsActive);
        Assert.Equal("Admin", booking.CreatedBy);
        Assert.Equal("Editor", booking.ModifiedBy);
        Assert.NotNull(booking.Tour);
        Assert.NotNull(booking.User);
    }

    [Fact]
    public void Booking_WithNavigationProperties_MaintainsRelationships()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Grand Canyon Adventure" };
        var user = new UserInfo { Email = "john@example.com", FirstName = "John", LastName = "Doe" };
        var booking = new Booking
        {
            Id = 1,
            TourId = 1,
            Email = "john@example.com",
            Tour = tour,
            User = user
        };

        // Act & Assert
        Assert.Equal(booking.TourId, booking.Tour.Id);
        Assert.Equal(booking.Email, booking.User.Email);
    }
}
