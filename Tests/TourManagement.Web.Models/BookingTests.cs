using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using TourManagement.Web.Models;

namespace TourManagement.Web.Models.Tests;

public class BookingTests
{
    [Fact]
    public void Booking_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var booking = new Booking();

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(0, booking.TourId);
        Assert.Null(booking.TourName);
        Assert.Null(booking.Place);
        Assert.Null(booking.Email);
        Assert.Null(booking.FirstName);
    }

    [Fact]
    public void Booking_TourId_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedId = 456;

        // Act
        booking.TourId = expectedId;

        // Assert
        Assert.Equal(expectedId, booking.TourId);
    }

    [Fact]
    public void Booking_TourName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedTourName = "Adventure Tour";

        // Act
        booking.TourName = expectedTourName;

        // Assert
        Assert.Equal(expectedTourName, booking.TourName);
    }

    [Fact]
    public void Booking_Place_SetAndGet_ReturnsCorrectValue()
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
    public void Booking_Email_SetAndGet_ReturnsCorrectValue()
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
    public void Booking_FirstName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var booking = new Booking();
        var expectedFirstName = "Alice";

        // Act
        booking.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, booking.FirstName);
    }

    [Fact]
    public void Booking_TourName_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourName = null;

        // Assert
        Assert.Null(booking.TourName);
    }

    [Fact]
    public void Booking_Place_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Place = null;

        // Assert
        Assert.Null(booking.Place);
    }

    [Fact]
    public void Booking_Email_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Email = null;

        // Assert
        Assert.Null(booking.Email);
    }

    [Fact]
    public void Booking_FirstName_CanBeNull()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.FirstName = null;

        // Assert
        Assert.Null(booking.FirstName);
    }

    [Fact]
    public void Booking_SetAllProperties_ReturnsExpectedValues()
    {
        // Arrange
        var booking = new Booking
        {
            TourId = 101,
            TourName = "Beach Tour",
            Place = "Maldives",
            Email = "john@example.com",
            FirstName = "John"
        };

        // Act & Assert
        Assert.Equal(101, booking.TourId);
        Assert.Equal("Beach Tour", booking.TourName);
        Assert.Equal("Maldives", booking.Place);
        Assert.Equal("john@example.com", booking.Email);
        Assert.Equal("John", booking.FirstName);
    }

    [Fact]
    public void Booking_MaxLength_TourName_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var booking = new Booking
        {
            TourName = new string('a', 51)
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Booking_MaxLength_Place_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var booking = new Booking
        {
            Place = new string('b', 51)
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Booking_MaxLength_Email_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var booking = new Booking
        {
            Email = new string('c', 51) + "@test.com"
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Booking_MaxLength_FirstName_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var booking = new Booking
        {
            FirstName = new string('d', 51)
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Booking_ValidObject_PassesValidation()
    {
        // Arrange
        var booking = new Booking
        {
            TourId = 1,
            TourName = "Europe Tour",
            Place = "Paris",
            Email = "test@example.com",
            FirstName = "John"
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    public void Booking_TourId_ValidValues(int tourId)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = tourId;

        // Assert
        Assert.Equal(tourId, booking.TourId);
    }

    [Theory]
    [InlineData("Mountain Tour")]
    [InlineData("City Tour")]
    [InlineData("Wildlife Safari")]
    public void Booking_TourName_ValidValues(string tourName)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourName = tourName;

        // Assert
        Assert.Equal(tourName, booking.TourName);
    }

    [Theory]
    [InlineData("New York")]
    [InlineData("Tokyo")]
    [InlineData("London")]
    public void Booking_Place_ValidValues(string place)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Place = place;

        // Assert
        Assert.Equal(place, booking.Place);
    }

    [Theory]
    [InlineData("user1@test.com")]
    [InlineData("user2@example.org")]
    [InlineData("admin@company.co")]
    public void Booking_Email_ValidValues(string email)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.Email = email;

        // Assert
        Assert.Equal(email, booking.Email);
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob")]
    [InlineData("Charlie")]
    public void Booking_FirstName_ValidValues(string firstName)
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.FirstName = firstName;

        // Assert
        Assert.Equal(firstName, booking.FirstName);
    }

    [Fact]
    public void Booking_TourId_NegativeValue()
    {
        // Arrange
        var booking = new Booking();

        // Act
        booking.TourId = -1;

        // Assert
        Assert.Equal(-1, booking.TourId);
    }

    [Fact]
    public void Booking_EmptyStrings_AreValid()
    {
        // Arrange
        var booking = new Booking
        {
            TourName = "",
            Place = "",
            Email = "",
            FirstName = ""
        };

        var context = new ValidationContext(booking);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(booking, context, results, true);

        // Assert
        Assert.True(isValid);
    }
}
