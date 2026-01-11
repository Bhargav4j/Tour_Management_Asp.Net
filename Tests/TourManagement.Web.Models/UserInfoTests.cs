using Xunit;
using System;
using System.ComponentModel.DataAnnotations;
using TourManagement.Web.Models;

namespace TourManagement.Web.Models.Tests;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.NotNull(userInfo);
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Equal(string.Empty, userInfo.FirstName);
        Assert.Equal(string.Empty, userInfo.LastName);
        Assert.Equal(string.Empty, userInfo.Gender);
        Assert.Equal(string.Empty, userInfo.Password);
        Assert.Equal(string.Empty, userInfo.Street);
        Assert.Equal(string.Empty, userInfo.City);
        Assert.Equal(string.Empty, userInfo.State);
    }

    [Fact]
    public void UserInfo_Email_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedEmail = "test@example.com";

        // Act
        userInfo.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, userInfo.Email);
    }

    [Fact]
    public void UserInfo_FirstName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedFirstName = "John";

        // Act
        userInfo.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, userInfo.FirstName);
    }

    [Fact]
    public void UserInfo_LastName_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedLastName = "Doe";

        // Act
        userInfo.LastName = expectedLastName;

        // Assert
        Assert.Equal(expectedLastName, userInfo.LastName);
    }

    [Fact]
    public void UserInfo_Gender_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedGender = "Male";

        // Act
        userInfo.Gender = expectedGender;

        // Assert
        Assert.Equal(expectedGender, userInfo.Gender);
    }

    [Fact]
    public void UserInfo_Password_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedPassword = "SecurePass123";

        // Act
        userInfo.Password = expectedPassword;

        // Assert
        Assert.Equal(expectedPassword, userInfo.Password);
    }

    [Fact]
    public void UserInfo_Dob_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedDob = new DateOnly(1990, 5, 15);

        // Act
        userInfo.Dob = expectedDob;

        // Assert
        Assert.Equal(expectedDob, userInfo.Dob);
    }

    [Fact]
    public void UserInfo_Street_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedStreet = "123 Main St";

        // Act
        userInfo.Street = expectedStreet;

        // Assert
        Assert.Equal(expectedStreet, userInfo.Street);
    }

    [Fact]
    public void UserInfo_City_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedCity = "New York";

        // Act
        userInfo.City = expectedCity;

        // Assert
        Assert.Equal(expectedCity, userInfo.City);
    }

    [Fact]
    public void UserInfo_State_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedState = "NY";

        // Act
        userInfo.State = expectedState;

        // Assert
        Assert.Equal(expectedState, userInfo.State);
    }

    [Fact]
    public void UserInfo_SetAllProperties_ReturnsExpectedValues()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateOnly(1985, 7, 20),
            Street = "456 Elm Street",
            City = "Los Angeles",
            State = "CA"
        };

        // Act & Assert
        Assert.Equal("john.doe@example.com", userInfo.Email);
        Assert.Equal("John", userInfo.FirstName);
        Assert.Equal("Doe", userInfo.LastName);
        Assert.Equal("Male", userInfo.Gender);
        Assert.Equal("password123", userInfo.Password);
        Assert.Equal(new DateOnly(1985, 7, 20), userInfo.Dob);
        Assert.Equal("456 Elm Street", userInfo.Street);
        Assert.Equal("Los Angeles", userInfo.City);
        Assert.Equal("CA", userInfo.State);
    }

    [Theory]
    [InlineData("test@test.com", "", "LastName", "Male", "password", "Street", "City", "State")]
    [InlineData("test@test.com", "FirstName", "", "Male", "password", "Street", "City", "State")]
    public void UserInfo_ValidationAttributes_EnforceRequiredFields(
        string email, string firstName, string lastName, string gender,
        string password, string street, string city, string state)
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            Password = password,
            Dob = new DateOnly(1990, 1, 1),
            Street = street,
            City = city,
            State = state
        };

        var context = new ValidationContext(userInfo);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(userInfo, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void UserInfo_MaxLength_Email_ExceedsLimit_ValidationFails()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = new string('a', 51) + "@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password",
            Dob = new DateOnly(1990, 1, 1),
            Street = "Street",
            City = "City",
            State = "State"
        };

        var context = new ValidationContext(userInfo);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(userInfo, context, results, true);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void UserInfo_ValidObject_PassesValidation()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = "valid@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateOnly(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        var context = new ValidationContext(userInfo);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(userInfo, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void UserInfo_Dob_EdgeCase_MinDate()
    {
        // Arrange
        var userInfo = new UserInfo();
        var minDate = DateOnly.MinValue;

        // Act
        userInfo.Dob = minDate;

        // Assert
        Assert.Equal(minDate, userInfo.Dob);
    }

    [Fact]
    public void UserInfo_Dob_EdgeCase_MaxDate()
    {
        // Arrange
        var userInfo = new UserInfo();
        var maxDate = DateOnly.MaxValue;

        // Act
        userInfo.Dob = maxDate;

        // Assert
        Assert.Equal(maxDate, userInfo.Dob);
    }
}
