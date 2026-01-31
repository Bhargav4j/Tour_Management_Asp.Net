using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Null(user.FullName);
        Assert.Null(user.Phone);
        Assert.Null(user.Address);
        Assert.False(user.IsAdmin);
        Assert.True(user.CreatedDate <= DateTime.UtcNow);
        Assert.Null(user.ModifiedDate);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Id_CanSetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedId = 123;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Theory]
    [InlineData("johndoe")]
    [InlineData("admin")]
    [InlineData("")]
    public void User_Username_CanSetAndGet(string username)
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = username;

        // Assert
        Assert.Equal(username, user.Username);
    }

    [Theory]
    [InlineData("john@example.com")]
    [InlineData("admin@test.com")]
    [InlineData("")]
    public void User_Email_CanSetAndGet(string email)
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = email;

        // Assert
        Assert.Equal(email, user.Email);
    }

    [Theory]
    [InlineData("hashedpassword123")]
    [InlineData("$2a$10$abcdefghijklmnopqrstuv")]
    [InlineData("")]
    public void User_PasswordHash_CanSetAndGet(string passwordHash)
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = passwordHash;

        // Assert
        Assert.Equal(passwordHash, user.PasswordHash);
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData(null)]
    [InlineData("")]
    public void User_FullName_CanSetAndGet(string? fullName)
    {
        // Arrange
        var user = new User();

        // Act
        user.FullName = fullName;

        // Assert
        Assert.Equal(fullName, user.FullName);
    }

    [Theory]
    [InlineData("123-456-7890")]
    [InlineData(null)]
    [InlineData("")]
    public void User_Phone_CanSetAndGet(string? phone)
    {
        // Arrange
        var user = new User();

        // Act
        user.Phone = phone;

        // Assert
        Assert.Equal(phone, user.Phone);
    }

    [Theory]
    [InlineData("123 Main Street")]
    [InlineData(null)]
    [InlineData("")]
    public void User_Address_CanSetAndGet(string? address)
    {
        // Arrange
        var user = new User();

        // Act
        user.Address = address;

        // Assert
        Assert.Equal(address, user.Address);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_IsAdmin_CanSetAndGet(bool isAdmin)
    {
        // Arrange
        var user = new User();

        // Act
        user.IsAdmin = isAdmin;

        // Assert
        Assert.Equal(isAdmin, user.IsAdmin);
    }

    [Fact]
    public void User_CreatedDate_CanSetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2024, 1, 1);

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void User_ModifiedDate_CanSetAndGetNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_ModifiedDate_CanSetAndGetValue()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_IsActive_CanSetAndGet(bool isActive)
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = isActive;

        // Assert
        Assert.Equal(isActive, user.IsActive);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User123")]
    [InlineData("")]
    public void User_CreatedBy_CanSetAndGet(string createdBy)
    {
        // Arrange
        var user = new User();

        // Act
        user.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, user.CreatedBy);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData(null)]
    [InlineData("")]
    public void User_ModifiedBy_CanSetAndGet(string? modifiedBy)
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void User_Bookings_CanSetAndGet()
    {
        // Arrange
        var user = new User();
        var bookings = new List<Booking>
        {
            new Booking { Id = 1 },
            new Booking { Id = 2 }
        };

        // Act
        user.Bookings = bookings;

        // Assert
        Assert.Equal(bookings, user.Bookings);
        Assert.Equal(2, user.Bookings.Count);
    }

    [Fact]
    public void User_Bookings_CanAddBooking()
    {
        // Arrange
        var user = new User();
        var booking = new Booking { Id = 1 };

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_AllProperties_CanSetAndGetCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedCreatedDate = new DateTime(2024, 1, 1);
        var expectedModifiedDate = new DateTime(2024, 6, 15);

        // Act
        user.Id = 50;
        user.Username = "testuser";
        user.Email = "test@example.com";
        user.PasswordHash = "hashed123";
        user.FullName = "Test User";
        user.Phone = "555-1234";
        user.Address = "123 Test St";
        user.IsAdmin = true;
        user.CreatedDate = expectedCreatedDate;
        user.ModifiedDate = expectedModifiedDate;
        user.IsActive = false;
        user.CreatedBy = "System";
        user.ModifiedBy = "Admin";

        // Assert
        Assert.Equal(50, user.Id);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hashed123", user.PasswordHash);
        Assert.Equal("Test User", user.FullName);
        Assert.Equal("555-1234", user.Phone);
        Assert.Equal("123 Test St", user.Address);
        Assert.True(user.IsAdmin);
        Assert.Equal(expectedCreatedDate, user.CreatedDate);
        Assert.Equal(expectedModifiedDate, user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Equal("Admin", user.ModifiedBy);
    }
}
