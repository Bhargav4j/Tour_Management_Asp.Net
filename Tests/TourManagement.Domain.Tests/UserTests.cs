using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
    }

    [Fact]
    public void UserId_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.UserId = 1;

        // Assert
        Assert.Equal(1, user.UserId);
    }

    [Fact]
    public void Email_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void Email_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void FirstName_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.FirstName = "John";

        // Assert
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void FirstName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.FirstName);
    }

    [Fact]
    public void LastName_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.LastName = "Doe";

        // Assert
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void LastName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.LastName);
    }

    [Fact]
    public void Gender_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Male";

        // Assert
        Assert.Equal("Male", user.Gender);
    }

    [Fact]
    public void Gender_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.Gender);
    }

    [Fact]
    public void PasswordHash_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "hashedpassword";

        // Assert
        Assert.Equal("hashedpassword", user.PasswordHash);
    }

    [Fact]
    public void PasswordHash_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.PasswordHash);
    }

    [Fact]
    public void DateOfBirth_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        user.DateOfBirth = dateOfBirth;

        // Assert
        Assert.Equal(dateOfBirth, user.DateOfBirth);
    }

    [Fact]
    public void Street_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Street = "123 Main St";

        // Assert
        Assert.Equal("123 Main St", user.Street);
    }

    [Fact]
    public void Street_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.Street);
    }

    [Fact]
    public void City_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.City = "New York";

        // Assert
        Assert.Equal("New York", user.City);
    }

    [Fact]
    public void City_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.City);
    }

    [Fact]
    public void State_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.State = "NY";

        // Assert
        Assert.Equal("NY", user.State);
    }

    [Fact]
    public void State_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(string.Empty, user.State);
    }

    [Fact]
    public void CreatedDate_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();
        var createdDate = DateTime.Now;

        // Act
        user.CreatedDate = createdDate;

        // Assert
        Assert.Equal(createdDate, user.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();
        var modifiedDate = DateTime.Now;

        // Act
        user.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, user.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_ShouldBeNullable()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_ShouldSetFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Bookings_ShouldInitializeAsEmptyList()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Bookings_ShouldAddBooking()
    {
        // Arrange
        var user = new User();
        var booking = new Booking { BookingId = 1, UserId = 1, TourId = 1 };

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_ShouldSetAllProperties()
    {
        // Arrange
        var user = new User
        {
            UserId = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "hash123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, user.UserId);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(new DateTime(1990, 1, 1), user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.True(user.IsActive);
    }
}
