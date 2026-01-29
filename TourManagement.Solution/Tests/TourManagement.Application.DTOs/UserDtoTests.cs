using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Tests.Application.DTOs;

public class UserDtoTests
{
    [Fact]
    public void UserDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new UserDto();

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
    public void UserDto_SetProperties_ShouldStoreValues()
    {
        var dto = new UserDto
        {
            Email = "user@example.com",
            FirstName = "Alice",
            LastName = "Smith",
            Gender = "Female",
            DateOfBirth = new DateTime(1995, 8, 10),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA",
            IsActive = true
        };

        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal("Alice", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal(new DateTime(1995, 8, 10), dto.DateOfBirth);
        Assert.Equal("456 Oak Ave", dto.Street);
        Assert.Equal("Los Angeles", dto.City);
        Assert.Equal("CA", dto.State);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserRegistrationDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new UserRegistrationDto();

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
    public void UserRegistrationDto_SetProperties_ShouldStoreValues()
    {
        var dto = new UserRegistrationDto
        {
            Email = "newuser@example.com",
            FirstName = "Bob",
            LastName = "Johnson",
            Gender = "Male",
            Password = "SecurePassword123",
            DateOfBirth = new DateTime(1988, 3, 25),
            Street = "789 Pine St",
            City = "Chicago",
            State = "IL"
        };

        Assert.Equal("newuser@example.com", dto.Email);
        Assert.Equal("Bob", dto.FirstName);
        Assert.Equal("Johnson", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("SecurePassword123", dto.Password);
        Assert.Equal(new DateTime(1988, 3, 25), dto.DateOfBirth);
        Assert.Equal("789 Pine St", dto.Street);
        Assert.Equal("Chicago", dto.City);
        Assert.Equal("IL", dto.State);
    }

    [Fact]
    public void UserLoginDto_Constructor_InitializesWithDefaultValues()
    {
        var dto = new UserLoginDto();

        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(string.Empty, dto.Password);
    }

    [Fact]
    public void UserLoginDto_SetProperties_ShouldStoreValues()
    {
        var dto = new UserLoginDto
        {
            Email = "login@example.com",
            Password = "MyPassword456"
        };

        Assert.Equal("login@example.com", dto.Email);
        Assert.Equal("MyPassword456", dto.Password);
    }

    [Fact]
    public void UserDto_DateOfBirth_CanBeSet()
    {
        var dto = new UserDto();
        var dateOfBirth = new DateTime(2000, 1, 1);

        dto.DateOfBirth = dateOfBirth;

        Assert.Equal(dateOfBirth, dto.DateOfBirth);
    }

    [Fact]
    public void UserRegistrationDto_DateOfBirth_CanBeSet()
    {
        var dto = new UserRegistrationDto();
        var dateOfBirth = new DateTime(1990, 12, 31);

        dto.DateOfBirth = dateOfBirth;

        Assert.Equal(dateOfBirth, dto.DateOfBirth);
    }
}
