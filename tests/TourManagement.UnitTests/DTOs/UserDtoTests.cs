using TourManagement.Domain.DTOs;
using Xunit;

namespace TourManagement.UnitTests.DTOs;

public class UserDtoTests
{
    [Fact]
    public void UserDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new UserDto();

        // Assert
        Assert.Equal(0, dto.Id);
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
    public void UserDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new UserDto();
        var dob = new DateTime(1990, 5, 15);
        var now = DateTime.UtcNow;

        // Act
        dto.Id = 1;
        dto.Email = "user@test.com";
        dto.FirstName = "Jane";
        dto.LastName = "Smith";
        dto.Gender = "Female";
        dto.DateOfBirth = dob;
        dto.Street = "456 Oak Ave";
        dto.City = "Boston";
        dto.State = "MA";
        dto.CreatedDate = now;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("user@test.com", dto.Email);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("456 Oak Ave", dto.Street);
        Assert.Equal("Boston", dto.City);
        Assert.Equal("MA", dto.State);
        Assert.Equal(now, dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserCreateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new UserCreateDto();

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

    [Fact]
    public void UserCreateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new UserCreateDto();
        var dob = new DateTime(1985, 3, 20);

        // Act
        dto.Email = "new@user.com";
        dto.FirstName = "Bob";
        dto.LastName = "Johnson";
        dto.Gender = "Male";
        dto.Password = "SecurePass123";
        dto.DateOfBirth = dob;
        dto.Street = "789 Elm St";
        dto.City = "Chicago";
        dto.State = "IL";

        // Assert
        Assert.Equal("new@user.com", dto.Email);
        Assert.Equal("Bob", dto.FirstName);
        Assert.Equal("Johnson", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("SecurePass123", dto.Password);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("789 Elm St", dto.Street);
        Assert.Equal("Chicago", dto.City);
        Assert.Equal("IL", dto.State);
    }

    [Fact]
    public void UserUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new UserUpdateDto();

        // Assert
        Assert.Equal(string.Empty, dto.FirstName);
        Assert.Equal(string.Empty, dto.LastName);
        Assert.Equal(string.Empty, dto.Gender);
        Assert.Equal(string.Empty, dto.Street);
        Assert.Equal(string.Empty, dto.City);
        Assert.Equal(string.Empty, dto.State);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void UserUpdateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new UserUpdateDto();
        var dob = new DateTime(1992, 7, 10);

        // Act
        dto.FirstName = "Alice";
        dto.LastName = "Williams";
        dto.Gender = "Female";
        dto.DateOfBirth = dob;
        dto.Street = "321 Pine Rd";
        dto.City = "Seattle";
        dto.State = "WA";
        dto.IsActive = true;

        // Assert
        Assert.Equal("Alice", dto.FirstName);
        Assert.Equal("Williams", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal(dob, dto.DateOfBirth);
        Assert.Equal("321 Pine Rd", dto.Street);
        Assert.Equal("Seattle", dto.City);
        Assert.Equal("WA", dto.State);
        Assert.True(dto.IsActive);
    }
}
