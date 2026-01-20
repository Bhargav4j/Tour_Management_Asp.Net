using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.Equal(0, userInfo.Id);
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Equal(string.Empty, userInfo.FirstName);
        Assert.Equal(string.Empty, userInfo.LastName);
        Assert.Equal(string.Empty, userInfo.Gender);
        Assert.Equal(string.Empty, userInfo.Password);
        Assert.Equal(default(DateTime), userInfo.DateOfBirth);
        Assert.Equal(string.Empty, userInfo.Street);
        Assert.Equal(string.Empty, userInfo.City);
        Assert.Equal(string.Empty, userInfo.State);
        Assert.True(userInfo.IsActive);
        Assert.Equal("System", userInfo.CreatedBy);
        Assert.Null(userInfo.ModifiedBy);
        Assert.Null(userInfo.ModifiedDate);
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_Id_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Id = 100;

        // Assert
        Assert.Equal(100, userInfo.Id);
    }

    [Fact]
    public void UserInfo_Email_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", userInfo.Email);
    }

    [Fact]
    public void UserInfo_FirstName_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.FirstName = "John";

        // Assert
        Assert.Equal("John", userInfo.FirstName);
    }

    [Fact]
    public void UserInfo_LastName_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.LastName = "Doe";

        // Assert
        Assert.Equal("Doe", userInfo.LastName);
    }

    [Fact]
    public void UserInfo_Gender_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Gender = "Male";

        // Assert
        Assert.Equal("Male", userInfo.Gender);
    }

    [Fact]
    public void UserInfo_Password_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Password = "SecurePassword123";

        // Assert
        Assert.Equal("SecurePassword123", userInfo.Password);
    }

    [Fact]
    public void UserInfo_DateOfBirth_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var dob = new DateTime(1990, 5, 15);

        // Act
        userInfo.DateOfBirth = dob;

        // Assert
        Assert.Equal(dob, userInfo.DateOfBirth);
    }

    [Fact]
    public void UserInfo_Street_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Street = "123 Main St";

        // Assert
        Assert.Equal("123 Main St", userInfo.Street);
    }

    [Fact]
    public void UserInfo_City_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.City = "New York";

        // Assert
        Assert.Equal("New York", userInfo.City);
    }

    [Fact]
    public void UserInfo_State_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.State = "NY";

        // Assert
        Assert.Equal("NY", userInfo.State);
    }

    [Fact]
    public void UserInfo_CreatedDate_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        userInfo.CreatedDate = date;

        // Assert
        Assert.Equal(date, userInfo.CreatedDate);
    }

    [Fact]
    public void UserInfo_ModifiedDate_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var date = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        userInfo.ModifiedDate = date;

        // Assert
        Assert.Equal(date, userInfo.ModifiedDate);
    }

    [Fact]
    public void UserInfo_IsActive_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.IsActive = false;

        // Assert
        Assert.False(userInfo.IsActive);
    }

    [Fact]
    public void UserInfo_CreatedBy_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", userInfo.CreatedBy);
    }

    [Fact]
    public void UserInfo_ModifiedBy_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.ModifiedBy = "User123";

        // Assert
        Assert.Equal("User123", userInfo.ModifiedBy);
    }

    [Fact]
    public void UserInfo_Bookings_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var bookings = new List<Booking> { new Booking(), new Booking(), new Booking() };

        // Act
        userInfo.Bookings = bookings;

        // Assert
        Assert.Equal(3, userInfo.Bookings.Count);
    }

    [Fact]
    public void UserInfo_AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var userInfo = new UserInfo
        {
            Id = 500,
            Email = "complete@test.com",
            FirstName = "Jane",
            LastName = "Smith",
            Gender = "Female",
            Password = "Password123",
            DateOfBirth = new DateTime(1985, 3, 20),
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "System",
            ModifiedBy = "Admin"
        };

        // Assert
        Assert.Equal(500, userInfo.Id);
        Assert.Equal("complete@test.com", userInfo.Email);
        Assert.Equal("Jane", userInfo.FirstName);
        Assert.Equal("Smith", userInfo.LastName);
        Assert.Equal("Female", userInfo.Gender);
        Assert.Equal("Password123", userInfo.Password);
        Assert.Equal(new DateTime(1985, 3, 20), userInfo.DateOfBirth);
        Assert.Equal("456 Oak Ave", userInfo.Street);
        Assert.Equal("Los Angeles", userInfo.City);
        Assert.Equal("CA", userInfo.State);
        Assert.True(userInfo.IsActive);
        Assert.Equal("System", userInfo.CreatedBy);
        Assert.Equal("Admin", userInfo.ModifiedBy);
    }
}
