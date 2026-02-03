using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserInfoTests
{
    [Fact]
    public void UserInfo_DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Equal(string.Empty, userInfo.FirstName);
        Assert.Equal(string.Empty, userInfo.LastName);
        Assert.Equal(string.Empty, userInfo.Gender);
        Assert.Equal(string.Empty, userInfo.Password);
        Assert.Equal(string.Empty, userInfo.Street);
        Assert.Equal(string.Empty, userInfo.City);
        Assert.Equal(string.Empty, userInfo.State);
        Assert.Equal(string.Empty, userInfo.CreatedBy);
        Assert.False(userInfo.IsActive);
        Assert.Null(userInfo.ModifiedBy);
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_SetProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var userInfo = new UserInfo();
        var dob = new DateTime(1990, 1, 1);
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        userInfo.Email = "test@example.com";
        userInfo.FirstName = "John";
        userInfo.LastName = "Doe";
        userInfo.Gender = "Male";
        userInfo.Password = "hashedpassword";
        userInfo.DateOfBirth = dob;
        userInfo.Street = "123 Main St";
        userInfo.City = "New York";
        userInfo.State = "NY";
        userInfo.CreatedDate = createdDate;
        userInfo.ModifiedDate = modifiedDate;
        userInfo.IsActive = true;
        userInfo.CreatedBy = "Admin";
        userInfo.ModifiedBy = "User";

        // Assert
        Assert.Equal("test@example.com", userInfo.Email);
        Assert.Equal("John", userInfo.FirstName);
        Assert.Equal("Doe", userInfo.LastName);
        Assert.Equal("Male", userInfo.Gender);
        Assert.Equal("hashedpassword", userInfo.Password);
        Assert.Equal(dob, userInfo.DateOfBirth);
        Assert.Equal("123 Main St", userInfo.Street);
        Assert.Equal("New York", userInfo.City);
        Assert.Equal("NY", userInfo.State);
        Assert.Equal(createdDate, userInfo.CreatedDate);
        Assert.Equal(modifiedDate, userInfo.ModifiedDate);
        Assert.True(userInfo.IsActive);
        Assert.Equal("Admin", userInfo.CreatedBy);
        Assert.Equal("User", userInfo.ModifiedBy);
    }

    [Fact]
    public void UserInfo_Bookings_ShouldBeEmptyCollectionByDefault()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_AddBooking_ShouldIncreaseBookingsCount()
    {
        // Arrange
        var userInfo = new UserInfo { Email = "test@example.com" };
        var booking = new Booking { Id = 1, Email = "test@example.com" };

        // Act
        userInfo.Bookings.Add(booking);

        // Assert
        Assert.Single(userInfo.Bookings);
        Assert.Contains(booking, userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_Email_ShouldAcceptValidEmail()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Email = "valid.email@example.com";

        // Assert
        Assert.Equal("valid.email@example.com", userInfo.Email);
    }

    [Fact]
    public void UserInfo_ModifiedDate_CanBeNull()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.ModifiedDate = null;

        // Assert
        Assert.Null(userInfo.ModifiedDate);
    }

    [Fact]
    public void UserInfo_ModifiedBy_CanBeNull()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.ModifiedBy = null;

        // Assert
        Assert.Null(userInfo.ModifiedBy);
    }

    [Fact]
    public void UserInfo_DateOfBirth_ShouldAcceptValidDate()
    {
        // Arrange
        var userInfo = new UserInfo();
        var dob = new DateTime(1985, 5, 15);

        // Act
        userInfo.DateOfBirth = dob;

        // Assert
        Assert.Equal(dob, userInfo.DateOfBirth);
    }
}
