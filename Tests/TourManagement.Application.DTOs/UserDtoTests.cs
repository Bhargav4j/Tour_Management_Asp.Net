using Xunit;
using System;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests;

public class UserDtoTests
{
    [Fact]
    public void UserDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new UserDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Null(dto.PhoneNumber);
        Assert.Equal(default(DateTime), dto.CreatedDate);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UserDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new UserDto
        {
            Id = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("+1234567890", dto.PhoneNumber);
        Assert.NotNull(dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_Email_CanBeEmpty()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Email);
    }

    [Fact]
    public void UserDto_PhoneNumber_CanBeNull()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.PhoneNumber = null;

        // Assert
        Assert.Null(dto.PhoneNumber);
    }

    [Fact]
    public void UserDto_SetId_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Id = -1;

        // Assert
        Assert.Equal(-1, dto.Id);
    }
}

public class UserCreateDtoTests
{
    [Fact]
    public void UserCreateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new UserCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Password);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Null(dto.PhoneNumber);
    }

    [Fact]
    public void UserCreateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "newuser@example.com",
            Password = "SecurePassword123",
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "+9876543210"
        };

        // Assert
        Assert.Equal("newuser@example.com", dto.Email);
        Assert.Equal("SecurePassword123", dto.Password);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("+9876543210", dto.PhoneNumber);
    }

    [Fact]
    public void UserCreateDto_Email_CanBeEmpty()
    {
        // Arrange
        var dto = new UserCreateDto();

        // Act
        dto.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Email);
    }

    [Fact]
    public void UserCreateDto_Password_CanBeEmpty()
    {
        // Arrange
        var dto = new UserCreateDto();

        // Act
        dto.Password = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void UserCreateDto_PhoneNumber_CanBeNull()
    {
        // Arrange
        var dto = new UserCreateDto();

        // Act
        dto.PhoneNumber = null;

        // Assert
        Assert.Null(dto.PhoneNumber);
    }

    [Fact]
    public void UserCreateDto_SetPassword_WithSpecialCharacters_SetsCorrectly()
    {
        // Arrange
        var dto = new UserCreateDto();
        var password = "P@ssw0rd!#$";

        // Act
        dto.Password = password;

        // Assert
        Assert.Equal(password, dto.Password);
    }
}

public class UserUpdateDtoTests
{
    [Fact]
    public void UserUpdateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new UserUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Null(dto.PhoneNumber);
    }

    [Fact]
    public void UserUpdateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new UserUpdateDto
        {
            Email = "updated@example.com",
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            PhoneNumber = "+1112223333"
        };

        // Assert
        Assert.Equal("updated@example.com", dto.Email);
        Assert.Equal("UpdatedFirst", dto.FirstName);
        Assert.Equal("UpdatedLast", dto.LastName);
        Assert.Equal("+1112223333", dto.PhoneNumber);
    }

    [Fact]
    public void UserUpdateDto_Email_CanBeEmpty()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Email);
    }

    [Fact]
    public void UserUpdateDto_PhoneNumber_CanBeNull()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.PhoneNumber = null;

        // Assert
        Assert.Null(dto.PhoneNumber);
    }

    [Fact]
    public void UserUpdateDto_FirstName_CanBeSet()
    {
        // Arrange
        var dto = new UserUpdateDto();
        var firstName = "Alice";

        // Act
        dto.FirstName = firstName;

        // Assert
        Assert.Equal(firstName, dto.FirstName);
    }
}
