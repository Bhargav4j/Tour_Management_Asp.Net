using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_DefaultValues_ShouldBeInitialized()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
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
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Id_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedId = 100;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void Email_SetValue_ShouldReturnValue()
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
    public void Email_SetEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void FirstName_SetValue_ShouldReturnValue()
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
    public void LastName_SetValue_ShouldReturnValue()
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
    public void Gender_SetValue_ShouldReturnValue()
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
    public void PasswordHash_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedHash = "hashed_password_123";

        // Act
        user.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, user.PasswordHash);
    }

    [Fact]
    public void DateOfBirth_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(1990, 5, 15);

        // Act
        user.DateOfBirth = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.DateOfBirth);
    }

    [Fact]
    public void Street_SetValue_ShouldReturnValue()
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
    public void City_SetValue_ShouldReturnValue()
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
    public void State_SetValue_ShouldReturnValue()
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
    public void CreatedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2026, 1, 27);

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_SetNull_ShouldReturnNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetTrue_ShouldReturnTrue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_SetFalse_ShouldReturnFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void CreatedBy_SetValue_ShouldReturnValue()
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
    public void ModifiedBy_SetValue_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var expectedModifiedBy = "Manager";

        // Act
        user.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void ModifiedBy_SetNull_ShouldReturnNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = null;

        // Assert
        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void Bookings_AddBooking_ShouldContainBooking()
    {
        // Arrange
        var user = new User();
        var booking = new Booking { Id = 1, UserId = 1 };

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void Bookings_AddMultipleBookings_ShouldContainAllBookings()
    {
        // Arrange
        var user = new User();
        var booking1 = new Booking { Id = 1, UserId = 1 };
        var booking2 = new Booking { Id = 2, UserId = 1 };

        // Act
        user.Bookings.Add(booking1);
        user.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, user.Bookings.Count);
        Assert.Contains(booking1, user.Bookings);
        Assert.Contains(booking2, user.Bookings);
    }

    [Fact]
    public void User_SetAllProperties_ShouldReturnAllValues()
    {
        // Arrange
        var user = new User();
        var expectedId = 1;
        var expectedEmail = "john.doe@example.com";
        var expectedFirstName = "John";
        var expectedLastName = "Doe";
        var expectedGender = "Male";
        var expectedPasswordHash = "hashed123";
        var expectedDateOfBirth = new DateTime(1990, 1, 1);
        var expectedStreet = "123 Main St";
        var expectedCity = "Boston";
        var expectedState = "MA";
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "TestUser";
        var expectedModifiedBy = "TestAdmin";

        // Act
        user.Id = expectedId;
        user.Email = expectedEmail;
        user.FirstName = expectedFirstName;
        user.LastName = expectedLastName;
        user.Gender = expectedGender;
        user.PasswordHash = expectedPasswordHash;
        user.DateOfBirth = expectedDateOfBirth;
        user.Street = expectedStreet;
        user.City = expectedCity;
        user.State = expectedState;
        user.CreatedDate = expectedCreatedDate;
        user.ModifiedDate = expectedModifiedDate;
        user.IsActive = expectedIsActive;
        user.CreatedBy = expectedCreatedBy;
        user.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedId, user.Id);
        Assert.Equal(expectedEmail, user.Email);
        Assert.Equal(expectedFirstName, user.FirstName);
        Assert.Equal(expectedLastName, user.LastName);
        Assert.Equal(expectedGender, user.Gender);
        Assert.Equal(expectedPasswordHash, user.PasswordHash);
        Assert.Equal(expectedDateOfBirth, user.DateOfBirth);
        Assert.Equal(expectedStreet, user.Street);
        Assert.Equal(expectedCity, user.City);
        Assert.Equal(expectedState, user.State);
        Assert.Equal(expectedCreatedDate, user.CreatedDate);
        Assert.Equal(expectedModifiedDate, user.ModifiedDate);
        Assert.Equal(expectedIsActive, user.IsActive);
        Assert.Equal(expectedCreatedBy, user.CreatedBy);
        Assert.Equal(expectedModifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void Email_SetMultipleValidFormats_ShouldReturnValue()
    {
        // Arrange
        var user = new User();
        var email1 = "user@domain.com";
        var email2 = "first.last@example.org";

        // Act & Assert
        user.Email = email1;
        Assert.Equal(email1, user.Email);

        user.Email = email2;
        Assert.Equal(email2, user.Email);
    }

    [Fact]
    public void Gender_SetDifferentValues_ShouldReturnValue()
    {
        // Arrange
        var user = new User();

        // Act & Assert
        user.Gender = "Male";
        Assert.Equal("Male", user.Gender);

        user.Gender = "Female";
        Assert.Equal("Female", user.Gender);

        user.Gender = "Other";
        Assert.Equal("Other", user.Gender);
    }
}
