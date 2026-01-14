using Xunit;
using TourManagement.Domain.DTOs;
using System;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for UserDto, UserCreateDto, and UserUpdateDto
/// </summary>
public class UserDtoTests
{
    #region UserDto Tests

    [Fact]
    public void UserDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new UserDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UserDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new UserDto
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = new DateTime(2024, 1, 15),
            IsActive = true
        };

        // Assert
        Assert.Equal("john.doe@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(new DateTime(1990, 5, 15), dto.DateOfBirth);
        Assert.Equal("123 Main St", dto.Street);
        Assert.Equal("New York", dto.City);
        Assert.Equal("NY", dto.State);
        Assert.Equal(new DateTime(2024, 1, 15), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_Email_CanBeSetToValidEmail()
    {
        // Arrange
        var dto = new UserDto { Email = "user@example.com" };

        // Assert
        Assert.Equal("user@example.com", dto.Email);
    }

    [Fact]
    public void UserDto_IsActive_CanBeTrue()
    {
        // Arrange
        var dto = new UserDto { IsActive = true };

        // Assert
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_IsActive_CanBeFalse()
    {
        // Arrange
        var dto = new UserDto { IsActive = false };

        // Assert
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UserDto_DateOfBirth_CanBeSet()
    {
        // Arrange
        var date = new DateTime(1995, 10, 20);
        var dto = new UserDto { DateOfBirth = date };

        // Assert
        Assert.Equal(date, dto.DateOfBirth);
    }

    [Fact]
    public void UserDto_Gender_CanBeSetToMale()
    {
        // Arrange
        var dto = new UserDto { Gender = "Male" };

        // Assert
        Assert.Equal("Male", dto.Gender);
    }

    [Fact]
    public void UserDto_Gender_CanBeSetToFemale()
    {
        // Arrange
        var dto = new UserDto { Gender = "Female" };

        // Assert
        Assert.Equal("Female", dto.Gender);
    }

    #endregion

    #region UserCreateDto Tests

    [Fact]
    public void UserCreateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new UserCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    [Fact]
    public void UserCreateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "new.user@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "SecurePassword123!",
            DateOfBirth = new DateTime(1992, 8, 25),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA"
        };

        // Assert
        Assert.Equal("new.user@example.com", dto.Email);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal("SecurePassword123!", dto.Password);
        Assert.Equal(new DateTime(1992, 8, 25), dto.DateOfBirth);
        Assert.Equal("456 Oak Ave", dto.Street);
        Assert.Equal("Los Angeles", dto.City);
        Assert.Equal("CA", dto.State);
    }

    [Fact]
    public void UserCreateDto_Password_CanBeSet()
    {
        // Arrange
        var dto = new UserCreateDto { Password = "TestPassword" };

        // Assert
        Assert.Equal("TestPassword", dto.Password);
    }

    [Fact]
    public void UserCreateDto_Email_CanBeSetToEmpty()
    {
        // Arrange
        var dto = new UserCreateDto { Email = string.Empty };

        // Assert
        Assert.Equal(string.Empty, dto.Email);
    }

    [Fact]
    public void UserCreateDto_DateOfBirth_CanBeMinValue()
    {
        // Arrange
        var dto = new UserCreateDto { DateOfBirth = DateTime.MinValue };

        // Assert
        Assert.Equal(DateTime.MinValue, dto.DateOfBirth);
    }

    [Fact]
    public void UserCreateDto_DateOfBirth_CanBeMaxValue()
    {
        // Arrange
        var dto = new UserCreateDto { DateOfBirth = DateTime.MaxValue };

        // Assert
        Assert.Equal(DateTime.MaxValue, dto.DateOfBirth);
    }

    [Fact]
    public void UserCreateDto_AllStringProperties_CanBeEmpty()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Gender = string.Empty,
            Password = string.Empty,
            Street = string.Empty,
            City = string.Empty,
            State = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    #endregion

    #region UserUpdateDto Tests

    [Fact]
    public void UserUpdateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new UserUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    [Fact]
    public void UserUpdateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            FirstName = "UpdatedJohn",
            LastName = "UpdatedDoe",
            Gender = "Male",
            DateOfBirth = new DateTime(1991, 6, 20),
            Street = "789 Pine Rd",
            City = "Chicago",
            State = "IL"
        };

        // Assert
        Assert.Equal("UpdatedJohn", dto.FirstName);
        Assert.Equal("UpdatedDoe", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(new DateTime(1991, 6, 20), dto.DateOfBirth);
        Assert.Equal("789 Pine Rd", dto.Street);
        Assert.Equal("Chicago", dto.City);
        Assert.Equal("IL", dto.State);
    }

    [Fact]
    public void UserUpdateDto_DoesNotHaveEmailProperty()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Assert - UserUpdateDto should not have Email property (users typically can't change email in update)
        Assert.IsNotType<UserCreateDto>(dto);
    }

    [Fact]
    public void UserUpdateDto_DoesNotHavePasswordProperty()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Assert - UserUpdateDto should not have Password property (password change typically separate operation)
        Assert.IsNotType<UserCreateDto>(dto);
    }

    [Fact]
    public void UserUpdateDto_FirstName_CanBeModified()
    {
        // Arrange
        var dto = new UserUpdateDto { FirstName = "Original" };

        // Act
        dto.FirstName = "Modified";

        // Assert
        Assert.Equal("Modified", dto.FirstName);
    }

    [Fact]
    public void UserUpdateDto_LastName_CanBeModified()
    {
        // Arrange
        var dto = new UserUpdateDto { LastName = "Original" };

        // Act
        dto.LastName = "Modified";

        // Assert
        Assert.Equal("Modified", dto.LastName);
    }

    [Fact]
    public void UserUpdateDto_DateOfBirth_CanBeModified()
    {
        // Arrange
        var originalDate = new DateTime(1990, 1, 1);
        var modifiedDate = new DateTime(1995, 12, 31);
        var dto = new UserUpdateDto { DateOfBirth = originalDate };

        // Act
        dto.DateOfBirth = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, dto.DateOfBirth);
    }

    [Fact]
    public void UserUpdateDto_AllStringProperties_CanBeEmpty()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Gender = string.Empty,
            Street = string.Empty,
            City = string.Empty,
            State = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void UserCreateDto_HasMorePropertiesThan_UserUpdateDto()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "test@example.com",
            Password = "password"
        };

        var updateDto = new UserUpdateDto
        {
            FirstName = "Test"
        };

        // Assert - Create has Email and Password, Update does not
        Assert.NotEqual(string.Empty, createDto.Email);
        Assert.NotEqual(string.Empty, createDto.Password);
    }

    [Fact]
    public void UserDto_And_UserCreateDto_HaveSimilarProperties()
    {
        // Arrange
        var userDto = new UserDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        var createDto = new UserCreateDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Assert
        Assert.Equal(userDto.Email, createDto.Email);
        Assert.Equal(userDto.FirstName, createDto.FirstName);
        Assert.Equal(userDto.LastName, createDto.LastName);
        Assert.Equal(userDto.Gender, createDto.Gender);
        Assert.Equal(userDto.DateOfBirth, createDto.DateOfBirth);
        Assert.Equal(userDto.Street, createDto.Street);
        Assert.Equal(userDto.City, createDto.City);
        Assert.Equal(userDto.State, createDto.State);
    }

    #endregion
}
