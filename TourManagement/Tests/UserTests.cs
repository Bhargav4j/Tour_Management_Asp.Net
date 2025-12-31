using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

/// <summary>
/// Unit tests for User entity
/// </summary>
public class UserTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.Equal("System", user.CreatedBy);
        Assert.False(user.IsActive);
        Assert.False(user.IsAdmin);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedId = 123;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void Email_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedEmail = "test@example.com";

        // Act
        user.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void Email_SetToEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void FirstName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedFirstName = "John";

        // Act
        user.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, user.FirstName);
    }

    [Fact]
    public void LastName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedLastName = "Doe";

        // Act
        user.LastName = expectedLastName;

        // Assert
        Assert.Equal(expectedLastName, user.LastName);
    }

    [Fact]
    public void Gender_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedGender = "Male";

        // Act
        user.Gender = expectedGender;

        // Assert
        Assert.Equal(expectedGender, user.Gender);
    }

    [Fact]
    public void PasswordHash_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedHash = "hashedPassword123";

        // Act
        user.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, user.PasswordHash);
    }

    [Fact]
    public void DateOfBirth_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(1990, 1, 1);

        // Act
        user.DateOfBirth = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.DateOfBirth);
    }

    [Fact]
    public void Street_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedStreet = "123 Main St";

        // Act
        user.Street = expectedStreet;

        // Assert
        Assert.Equal(expectedStreet, user.Street);
    }

    [Fact]
    public void City_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedCity = "New York";

        // Act
        user.City = expectedCity;

        // Assert
        Assert.Equal(expectedCity, user.City);
    }

    [Fact]
    public void State_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedState = "NY";

        // Act
        user.State = expectedState;

        // Assert
        Assert.Equal(expectedState, user.State);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.UtcNow;

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.UtcNow;

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_SetToNull_ReturnsNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToTrue_ReturnsTrue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_SetToFalse_ReturnsFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedCreatedBy = "Admin";

        // Act
        user.CreatedBy = expectedCreatedBy;

        // Assert
        Assert.Equal(expectedCreatedBy, user.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var user = new User();
        var expectedModifiedBy = "Admin";

        // Act
        user.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void IsAdmin_SetToTrue_ReturnsTrue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsAdmin = true;

        // Assert
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public void IsAdmin_SetToFalse_ReturnsFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsAdmin = false;

        // Assert
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void Bookings_AddBooking_ContainsBooking()
    {
        // Arrange
        var user = new User();
        var booking = new Booking { Id = 1, TourName = "Test Tour" };

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void Bookings_AddMultipleBookings_ContainsAllBookings()
    {
        // Arrange
        var user = new User();
        var booking1 = new Booking { Id = 1, TourName = "Tour 1" };
        var booking2 = new Booking { Id = 2, TourName = "Tour 2" };

        // Act
        user.Bookings.Add(booking1);
        user.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, user.Bookings.Count);
        Assert.Contains(booking1, user.Bookings);
        Assert.Contains(booking2, user.Bookings);
    }

    [Fact]
    public void User_WithAllPropertiesSet_MaintainsValues()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 1,
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "hashedPassword",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "Admin",
            IsAdmin = false
        };

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashedPassword", user.PasswordHash);
        Assert.Equal(new DateTime(1990, 5, 15), user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Equal("Admin", user.ModifiedBy);
        Assert.False(user.IsAdmin);
        Assert.NotNull(user.CreatedDate);
        Assert.NotNull(user.ModifiedDate);
    }
}
