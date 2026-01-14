using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for UserInfo entity
/// </summary>
public class UserInfoTests
{
    [Fact]
    public void Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.NotNull(userInfo);
        Assert.Equal(string.Empty, userInfo.Email);
        Assert.Equal(string.Empty, userInfo.FirstName);
        Assert.Equal(string.Empty, userInfo.LastName);
        Assert.Equal(string.Empty, userInfo.Gender);
        Assert.Equal(string.Empty, userInfo.PasswordHash);
        Assert.Equal(string.Empty, userInfo.Street);
        Assert.Equal(string.Empty, userInfo.City);
        Assert.Equal(string.Empty, userInfo.State);
        Assert.True(userInfo.IsActive);
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
    }

    [Fact]
    public void Email_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedEmail = "test@example.com";

        // Act
        userInfo.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, userInfo.Email);
    }

    [Fact]
    public void Email_EmptyString_IsValid()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, userInfo.Email);
    }

    [Fact]
    public void FirstName_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedFirstName = "John";

        // Act
        userInfo.FirstName = expectedFirstName;

        // Assert
        Assert.Equal(expectedFirstName, userInfo.FirstName);
    }

    [Fact]
    public void LastName_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedLastName = "Doe";

        // Act
        userInfo.LastName = expectedLastName;

        // Assert
        Assert.Equal(expectedLastName, userInfo.LastName);
    }

    [Fact]
    public void Gender_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedGender = "Male";

        // Act
        userInfo.Gender = expectedGender;

        // Assert
        Assert.Equal(expectedGender, userInfo.Gender);
    }

    [Fact]
    public void Gender_CanBeSetToFemale()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Gender = "Female";

        // Assert
        Assert.Equal("Female", userInfo.Gender);
    }

    [Fact]
    public void Gender_CanBeSetToOther()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Gender = "Other";

        // Assert
        Assert.Equal("Other", userInfo.Gender);
    }

    [Fact]
    public void PasswordHash_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedHash = "$2a$10$abcdefghijklmnopqrstuv";

        // Act
        userInfo.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, userInfo.PasswordHash);
    }

    [Fact]
    public void PasswordHash_EmptyString_IsValid()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.PasswordHash = string.Empty;

        // Assert
        Assert.Equal(string.Empty, userInfo.PasswordHash);
    }

    [Fact]
    public void DateOfBirth_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedDate = new DateTime(1990, 5, 15);

        // Act
        userInfo.DateOfBirth = expectedDate;

        // Assert
        Assert.Equal(expectedDate, userInfo.DateOfBirth);
    }

    [Fact]
    public void DateOfBirth_MinValue_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.DateOfBirth = DateTime.MinValue;

        // Assert
        Assert.Equal(DateTime.MinValue, userInfo.DateOfBirth);
    }

    [Fact]
    public void DateOfBirth_MaxValue_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.DateOfBirth = DateTime.MaxValue;

        // Assert
        Assert.Equal(DateTime.MaxValue, userInfo.DateOfBirth);
    }

    [Fact]
    public void Street_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedStreet = "123 Main St";

        // Act
        userInfo.Street = expectedStreet;

        // Assert
        Assert.Equal(expectedStreet, userInfo.Street);
    }

    [Fact]
    public void City_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedCity = "New York";

        // Act
        userInfo.City = expectedCity;

        // Assert
        Assert.Equal(expectedCity, userInfo.City);
    }

    [Fact]
    public void State_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedState = "NY";

        // Act
        userInfo.State = expectedState;

        // Assert
        Assert.Equal(expectedState, userInfo.State);
    }

    [Fact]
    public void CreatedDate_CanBeSetAndGet()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedDate = new DateTime(2024, 1, 15);

        // Act
        userInfo.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, userInfo.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToNull()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.ModifiedDate = null;

        // Assert
        Assert.Null(userInfo.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_CanBeSetToValue()
    {
        // Arrange
        var userInfo = new UserInfo();
        var expectedDate = new DateTime(2024, 2, 20);

        // Act
        userInfo.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, userInfo.ModifiedDate);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.True(userInfo.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.IsActive = false;

        // Assert
        Assert.False(userInfo.IsActive);
    }

    [Fact]
    public void Bookings_InitializesAsEmptyList()
    {
        // Arrange & Act
        var userInfo = new UserInfo();

        // Assert
        Assert.NotNull(userInfo.Bookings);
        Assert.Empty(userInfo.Bookings);
    }

    [Fact]
    public void Bookings_CanAddBooking()
    {
        // Arrange
        var userInfo = new UserInfo();
        var booking = new Booking { Id = 1 };

        // Act
        userInfo.Bookings.Add(booking);

        // Assert
        Assert.Single(userInfo.Bookings);
        Assert.Contains(booking, userInfo.Bookings);
    }

    [Fact]
    public void Bookings_CanAddMultipleBookings()
    {
        // Arrange
        var userInfo = new UserInfo();
        var booking1 = new Booking { Id = 1 };
        var booking2 = new Booking { Id = 2 };

        // Act
        userInfo.Bookings.Add(booking1);
        userInfo.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, userInfo.Bookings.Count);
        Assert.Contains(booking1, userInfo.Bookings);
        Assert.Contains(booking2, userInfo.Bookings);
    }

    [Fact]
    public void Bookings_CanBeReplacedWithNewCollection()
    {
        // Arrange
        var userInfo = new UserInfo();
        var newBookings = new List<Booking> { new Booking { Id = 1 }, new Booking { Id = 2 } };

        // Act
        userInfo.Bookings = newBookings;

        // Assert
        Assert.Equal(2, userInfo.Bookings.Count);
        Assert.Equal(newBookings, userInfo.Bookings);
    }

    [Fact]
    public void UserInfo_AllPropertiesSet_MaintainsValues()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "$2a$10$hashedpassword",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = new DateTime(2024, 1, 1),
            ModifiedDate = new DateTime(2024, 1, 15),
            IsActive = true
        };

        // Act & Assert
        Assert.Equal("john.doe@example.com", userInfo.Email);
        Assert.Equal("John", userInfo.FirstName);
        Assert.Equal("Doe", userInfo.LastName);
        Assert.Equal("Male", userInfo.Gender);
        Assert.Equal("$2a$10$hashedpassword", userInfo.PasswordHash);
        Assert.Equal(new DateTime(1990, 5, 15), userInfo.DateOfBirth);
        Assert.Equal("123 Main St", userInfo.Street);
        Assert.Equal("New York", userInfo.City);
        Assert.Equal("NY", userInfo.State);
        Assert.Equal(new DateTime(2024, 1, 1), userInfo.CreatedDate);
        Assert.Equal(new DateTime(2024, 1, 15), userInfo.ModifiedDate);
        Assert.True(userInfo.IsActive);
    }

    [Fact]
    public void UserInfo_WithNullablePropertiesNull_MaintainsNullValues()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            Email = "test@example.com",
            ModifiedDate = null
        };

        // Act & Assert
        Assert.Null(userInfo.ModifiedDate);
    }

    [Fact]
    public void Email_ValidEmailFormat_CanBeSet()
    {
        // Arrange
        var userInfo = new UserInfo();

        // Act
        userInfo.Email = "user.name+tag@example.co.uk";

        // Assert
        Assert.Equal("user.name+tag@example.co.uk", userInfo.Email);
    }

    [Fact]
    public void FullName_CanBeConcatenated()
    {
        // Arrange
        var userInfo = new UserInfo
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var fullName = $"{userInfo.FirstName} {userInfo.LastName}";

        // Assert
        Assert.Equal("John Doe", fullName);
    }
}
