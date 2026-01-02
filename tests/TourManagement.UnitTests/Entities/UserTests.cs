using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldInitializeWithDefaultValues()
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
        Assert.False(user.IsActive);
        Assert.Equal(string.Empty, user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Properties_ShouldBeSettable()
    {
        // Arrange
        var user = new User();
        var dob = new DateTime(1990, 1, 1);
        var now = DateTime.UtcNow;

        // Act
        user.Id = 1;
        user.Email = "test@example.com";
        user.FirstName = "John";
        user.LastName = "Doe";
        user.Gender = "Male";
        user.PasswordHash = "hashed_password";
        user.DateOfBirth = dob;
        user.Street = "123 Main St";
        user.City = "New York";
        user.State = "NY";
        user.CreatedDate = now;
        user.ModifiedDate = now;
        user.IsActive = true;
        user.CreatedBy = "system";
        user.ModifiedBy = "admin";

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Equal(dob, user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.Equal(now, user.CreatedDate);
        Assert.Equal(now, user.ModifiedDate);
        Assert.True(user.IsActive);
        Assert.Equal("system", user.CreatedBy);
        Assert.Equal("admin", user.ModifiedBy);
    }

    [Fact]
    public void User_Email_ShouldAcceptValidFormat()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "user@test.com";

        // Assert
        Assert.Equal("user@test.com", user.Email);
    }

    [Fact]
    public void User_Bookings_ShouldAllowAddingBookings()
    {
        // Arrange
        var user = new User { Id = 1 };
        var booking = new Booking { Id = 1, UserId = 1 };

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_Gender_ShouldAcceptMaleValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Male";

        // Assert
        Assert.Equal("Male", user.Gender);
    }

    [Fact]
    public void User_Gender_ShouldAcceptFemaleValue()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Female";

        // Assert
        Assert.Equal("Female", user.Gender);
    }

    [Fact]
    public void User_ModifiedBy_CanBeNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = null;

        // Assert
        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void User_ModifiedDate_CanBeNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_IsActive_ShouldToggle()
    {
        // Arrange
        var user = new User { IsActive = false };

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_DateOfBirth_ShouldAcceptPastDates()
    {
        // Arrange
        var user = new User();
        var pastDate = new DateTime(1985, 5, 15);

        // Act
        user.DateOfBirth = pastDate;

        // Assert
        Assert.Equal(pastDate, user.DateOfBirth);
    }
}
