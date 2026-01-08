using Xunit;
using System;
using System.Collections.Generic;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_SetsDefaultValues()
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
        Assert.Equal(default(DateTime), user.DateOfBirth);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
    }

    [Fact]
    public void User_Id_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedId = 1;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void User_Email_SetAndGet()
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
    public void User_Email_EmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void User_FirstName_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedName = "John";

        // Act
        user.FirstName = expectedName;

        // Assert
        Assert.Equal(expectedName, user.FirstName);
    }

    [Fact]
    public void User_LastName_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedName = "Doe";

        // Act
        user.LastName = expectedName;

        // Assert
        Assert.Equal(expectedName, user.LastName);
    }

    [Fact]
    public void User_Gender_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedGender = "Male";

        // Act
        user.Gender = expectedGender;

        // Assert
        Assert.Equal(expectedGender, user.Gender);
    }

    [Fact]
    public void User_PasswordHash_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedHash = "$2a$11$abcdefghijklmnopqrstuv";

        // Act
        user.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, user.PasswordHash);
    }

    [Fact]
    public void User_DateOfBirth_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(1990, 1, 1);

        // Act
        user.DateOfBirth = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.DateOfBirth);
    }

    [Fact]
    public void User_Street_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedStreet = "123 Main St";

        // Act
        user.Street = expectedStreet;

        // Assert
        Assert.Equal(expectedStreet, user.Street);
    }

    [Fact]
    public void User_City_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedCity = "New York";

        // Act
        user.City = expectedCity;

        // Assert
        Assert.Equal(expectedCity, user.City);
    }

    [Fact]
    public void User_State_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedState = "NY";

        // Act
        user.State = expectedState;

        // Assert
        Assert.Equal(expectedState, user.State);
    }

    [Fact]
    public void User_CreatedDate_SetAndGet()
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
    public void User_ModifiedDate_SetAndGet()
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
    public void User_ModifiedDate_Null()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_IsActive_SetAndGet()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_IsActive_DefaultValue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_CreatedBy_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedCreator = "Admin";

        // Act
        user.CreatedBy = expectedCreator;

        // Assert
        Assert.Equal(expectedCreator, user.CreatedBy);
    }

    [Fact]
    public void User_ModifiedBy_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedModifier = "Admin";

        // Act
        user.ModifiedBy = expectedModifier;

        // Assert
        Assert.Equal(expectedModifier, user.ModifiedBy);
    }

    [Fact]
    public void User_ModifiedBy_Null()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = null;

        // Assert
        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void User_Bookings_InitializedAsEmptyList()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Bookings_CanAddBooking()
    {
        // Arrange
        var user = new User();
        var booking = new Booking();

        // Act
        user.Bookings.Add(booking);

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_AllProperties_SetAndGet()
    {
        // Arrange
        var user = new User();
        var expectedId = 1;
        var expectedEmail = "john.doe@example.com";
        var expectedFirstName = "John";
        var expectedLastName = "Doe";
        var expectedGender = "Male";
        var expectedPasswordHash = "$2a$11$hashedpassword";
        var expectedDateOfBirth = new DateTime(1990, 5, 15);
        var expectedStreet = "123 Main St";
        var expectedCity = "New York";
        var expectedState = "NY";
        var expectedCreatedDate = DateTime.Now;
        var expectedModifiedDate = DateTime.Now.AddDays(1);
        var expectedIsActive = true;
        var expectedCreatedBy = "System";
        var expectedModifiedBy = "Admin";

        // Act
        user.Id = expectedId;
        user.Email = expectedEmail;
        user.FirstName = expectedFirstName;
        user.LastName = expectedLastName;
        user.Gender = expectedGender;
        user.PasswordHash = expectedPasswordHash;
        user.DateOfBirth = expectedDateOfBirth;
        user.Street = expectedStreet;
        user.City = expectedCity;
        user.State = expectedState;
        user.CreatedDate = expectedCreatedDate;
        user.ModifiedDate = expectedModifiedDate;
        user.IsActive = expectedIsActive;
        user.CreatedBy = expectedCreatedBy;
        user.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedId, user.Id);
        Assert.Equal(expectedEmail, user.Email);
        Assert.Equal(expectedFirstName, user.FirstName);
        Assert.Equal(expectedLastName, user.LastName);
        Assert.Equal(expectedGender, user.Gender);
        Assert.Equal(expectedPasswordHash, user.PasswordHash);
        Assert.Equal(expectedDateOfBirth, user.DateOfBirth);
        Assert.Equal(expectedStreet, user.Street);
        Assert.Equal(expectedCity, user.City);
        Assert.Equal(expectedState, user.State);
        Assert.Equal(expectedCreatedDate, user.CreatedDate);
        Assert.Equal(expectedModifiedDate, user.ModifiedDate);
        Assert.Equal(expectedIsActive, user.IsActive);
        Assert.Equal(expectedCreatedBy, user.CreatedBy);
        Assert.Equal(expectedModifiedBy, user.ModifiedBy);
    }
}
