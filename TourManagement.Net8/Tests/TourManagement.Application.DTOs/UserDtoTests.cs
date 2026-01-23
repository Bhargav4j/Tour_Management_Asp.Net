using Xunit;
using TourManagement.Application.DTOs;
using System;

namespace TourManagement.Application.DTOs.Tests;

public class UserDtoTests
{
    [Fact]
    public void UserDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new UserDto();
        var date = DateTime.Now;
        var dob = new DateTime(1990, 1, 1);

        // Act
        dto.Id = 1;
        dto.Email = "test@test.com";
        dto.FirstName = "John";
        dto.LastName = "Doe";
        dto.Gender = "Male";
        dto.DateOfBirth = dob;
        dto.Street = "123 Main St";
        dto.City = "New York";
        dto.State = "NY";
        dto.CreatedDate = date;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("test@test.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("123 Main St", dto.Street);
        Assert.Equal("New York", dto.City);
        Assert.Equal("NY", dto.State);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserCreateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new UserCreateDto();
        var dob = new DateTime(1990, 1, 1);

        // Act
        dto.Email = "new@test.com";
        dto.FirstName = "Jane";
        dto.LastName = "Smith";
        dto.Gender = "Female";
        dto.Password = "password123";
        dto.DateOfBirth = dob;
        dto.Street = "456 Oak Ave";
        dto.City = "Boston";
        dto.State = "MA";

        // Assert
        Assert.Equal("new@test.com", dto.Email);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal("password123", dto.Password);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("456 Oak Ave", dto.Street);
        Assert.Equal("Boston", dto.City);
        Assert.Equal("MA", dto.State);
    }

    [Fact]
    public void UserUpdateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new UserUpdateDto();
        var dob = new DateTime(1985, 5, 15);

        // Act
        dto.FirstName = "Updated";
        dto.LastName = "User";
        dto.Gender = "Male";
        dto.DateOfBirth = dob;
        dto.Street = "789 Pine St";
        dto.City = "Chicago";
        dto.State = "IL";

        // Assert
        Assert.Equal("Updated", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("789 Pine St", dto.Street);
        Assert.Equal("Chicago", dto.City);
        Assert.Equal("IL", dto.State);
    }
}
