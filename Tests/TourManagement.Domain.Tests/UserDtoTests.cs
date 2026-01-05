using Xunit;
using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.DTOs.Tests;

public class UserDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new UserDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void UserId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.UserId = 1;

        // Assert
        Assert.Equal(1, dto.UserId);
    }

    [Fact]
    public void Email_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", dto.Email);
    }

    [Fact]
    public void FirstName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.FirstName = "John";

        // Assert
        Assert.Equal("John", dto.FirstName);
    }

    [Fact]
    public void LastName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.LastName = "Doe";

        // Assert
        Assert.Equal("Doe", dto.LastName);
    }

    [Fact]
    public void Gender_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Gender = "Male";

        // Assert
        Assert.Equal("Male", dto.Gender);
    }

    [Fact]
    public void DateOfBirth_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();
        var date = new DateTime(1990, 1, 1);

        // Act
        dto.DateOfBirth = date;

        // Assert
        Assert.Equal(date, dto.DateOfBirth);
    }

    [Fact]
    public void Street_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.Street = "123 Main St";

        // Assert
        Assert.Equal("123 Main St", dto.Street);
    }

    [Fact]
    public void City_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.City = "New York";

        // Assert
        Assert.Equal("New York", dto.City);
    }

    [Fact]
    public void State_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.State = "NY";

        // Assert
        Assert.Equal("NY", dto.State);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserDto();

        // Act
        dto.IsActive = true;

        // Assert
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void UserDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new UserDto
        {
            UserId = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.UserId);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal(new DateTime(1990, 1, 1), dto.DateOfBirth);
        Assert.Equal("123 Main St", dto.Street);
        Assert.Equal("New York", dto.City);
        Assert.Equal("NY", dto.State);
        Assert.True(dto.IsActive);
    }
}

public class UserRegisterDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new UserRegisterDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void Email_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.Email = "register@example.com";

        // Assert
        Assert.Equal("register@example.com", dto.Email);
    }

    [Fact]
    public void FirstName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.FirstName = "Jane";

        // Assert
        Assert.Equal("Jane", dto.FirstName);
    }

    [Fact]
    public void LastName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.LastName = "Smith";

        // Assert
        Assert.Equal("Smith", dto.LastName);
    }

    [Fact]
    public void Gender_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.Gender = "Female";

        // Assert
        Assert.Equal("Female", dto.Gender);
    }

    [Fact]
    public void Password_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.Password = "SecurePassword123";

        // Assert
        Assert.Equal("SecurePassword123", dto.Password);
    }

    [Fact]
    public void DateOfBirth_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();
        var date = new DateTime(1995, 5, 15);

        // Act
        dto.DateOfBirth = date;

        // Assert
        Assert.Equal(date, dto.DateOfBirth);
    }

    [Fact]
    public void Street_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.Street = "456 Oak Ave";

        // Assert
        Assert.Equal("456 Oak Ave", dto.Street);
    }

    [Fact]
    public void City_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.City = "Los Angeles";

        // Assert
        Assert.Equal("Los Angeles", dto.City);
    }

    [Fact]
    public void State_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserRegisterDto();

        // Act
        dto.State = "CA";

        // Assert
        Assert.Equal("CA", dto.State);
    }

    [Fact]
    public void UserRegisterDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new UserRegisterDto
        {
            Email = "register@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "SecurePassword123",
            DateOfBirth = new DateTime(1995, 5, 15),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA"
        };

        // Assert
        Assert.Equal("register@example.com", dto.Email);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Smith", dto.LastName);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal("SecurePassword123", dto.Password);
        Assert.Equal(new DateTime(1995, 5, 15), dto.DateOfBirth);
        Assert.Equal("456 Oak Ave", dto.Street);
        Assert.Equal("Los Angeles", dto.City);
        Assert.Equal("CA", dto.State);
    }
}

public class UserLoginDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new UserLoginDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void Email_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserLoginDto();

        // Act
        dto.Email = "login@example.com";

        // Assert
        Assert.Equal("login@example.com", dto.Email);
    }

    [Fact]
    public void Password_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserLoginDto();

        // Act
        dto.Password = "MyPassword123";

        // Assert
        Assert.Equal("MyPassword123", dto.Password);
    }

    [Fact]
    public void UserLoginDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new UserLoginDto
        {
            Email = "login@example.com",
            Password = "MyPassword123"
        };

        // Assert
        Assert.Equal("login@example.com", dto.Email);
        Assert.Equal("MyPassword123", dto.Password);
    }

    [Fact]
    public void Email_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new UserLoginDto();

        // Assert
        Assert.Equal(string.Empty, dto.Email);
    }

    [Fact]
    public void Password_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new UserLoginDto();

        // Assert
        Assert.Equal(string.Empty, dto.Password);
    }
}

public class UserUpdateDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new UserUpdateDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void FirstName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.FirstName = "Updated";

        // Assert
        Assert.Equal("Updated", dto.FirstName);
    }

    [Fact]
    public void LastName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.LastName = "User";

        // Assert
        Assert.Equal("User", dto.LastName);
    }

    [Fact]
    public void Gender_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.Gender = "Other";

        // Assert
        Assert.Equal("Other", dto.Gender);
    }

    [Fact]
    public void DateOfBirth_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();
        var date = new DateTime(1985, 3, 20);

        // Act
        dto.DateOfBirth = date;

        // Assert
        Assert.Equal(date, dto.DateOfBirth);
    }

    [Fact]
    public void Street_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.Street = "789 Pine Rd";

        // Assert
        Assert.Equal("789 Pine Rd", dto.Street);
    }

    [Fact]
    public void City_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.City = "Chicago";

        // Assert
        Assert.Equal("Chicago", dto.City);
    }

    [Fact]
    public void State_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new UserUpdateDto();

        // Act
        dto.State = "IL";

        // Assert
        Assert.Equal("IL", dto.State);
    }

    [Fact]
    public void UserUpdateDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new UserUpdateDto
        {
            FirstName = "Updated",
            LastName = "User",
            Gender = "Other",
            DateOfBirth = new DateTime(1985, 3, 20),
            Street = "789 Pine Rd",
            City = "Chicago",
            State = "IL"
        };

        // Assert
        Assert.Equal("Updated", dto.FirstName);
        Assert.Equal("User", dto.LastName);
        Assert.Equal("Other", dto.Gender);
        Assert.Equal(new DateTime(1985, 3, 20), dto.DateOfBirth);
        Assert.Equal("789 Pine Rd", dto.Street);
        Assert.Equal("Chicago", dto.City);
        Assert.Equal("IL", dto.State);
    }
}
