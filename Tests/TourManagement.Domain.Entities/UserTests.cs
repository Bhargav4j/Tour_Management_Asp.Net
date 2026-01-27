using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Null(user.PhoneNumber);
        Assert.Equal(default(DateTime), user.CreatedDate);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.Equal(string.Empty, user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
    }

    [Fact]
    public void User_SetId_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedId = 456;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void User_SetEmail_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedEmail = "test@example.com";

        // Act
        user.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void User_SetPasswordHash_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedHash = "hashedpassword123";

        // Act
        user.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, user.PasswordHash);
    }

    [Fact]
    public void User_SetFirstName_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedFirstName = "John";

        // Act
        user.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, user.FirstName);
    }

    [Fact]
    public void User_SetLastName_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedLastName = "Doe";

        // Act
        user.LastName = expectedLastName;

        // Assert
        Assert.Equal(expectedLastName, user.LastName);
    }

    [Fact]
    public void User_SetPhoneNumber_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedPhone = "+1234567890";

        // Act
        user.PhoneNumber = expectedPhone;

        // Assert
        Assert.Equal(expectedPhone, user.PhoneNumber);
    }

    [Fact]
    public void User_SetCreatedDate_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.Now;

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void User_SetModifiedDate_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.Now;

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Fact]
    public void User_SetIsActive_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_SetCreatedBy_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedUser = "admin";

        // Act
        user.CreatedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, user.CreatedBy);
    }

    [Fact]
    public void User_SetModifiedBy_SetsValueCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedUser = "admin";

        // Act
        user.ModifiedBy = expectedUser;

        // Assert
        Assert.Equal(expectedUser, user.ModifiedBy);
    }

    [Fact]
    public void User_Bookings_DefaultsToEmptyList()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Bookings_CanAddBookings()
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
    public void User_Email_CanBeEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void User_AllPropertiesSet_RetainsValues()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = "hash123",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsActive = true,
            CreatedBy = "admin",
            ModifiedBy = "user"
        };

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("+1234567890", user.PhoneNumber);
        Assert.NotNull(user.CreatedDate);
        Assert.NotNull(user.ModifiedDate);
        Assert.True(user.IsActive);
        Assert.Equal("admin", user.CreatedBy);
        Assert.Equal("user", user.ModifiedBy);
    }

    [Fact]
    public void User_PhoneNumber_CanBeNull()
    {
        // Arrange
        var user = new User();

        // Act
        user.PhoneNumber = null;

        // Assert
        Assert.Null(user.PhoneNumber);
    }
}
