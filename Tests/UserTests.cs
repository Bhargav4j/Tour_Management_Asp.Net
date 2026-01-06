using System;
using System.Collections.Generic;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
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
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Id_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 1;

        // Assert
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public void Email_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void FirstName_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.FirstName = "John";

        // Assert
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void LastName_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.LastName = "Doe";

        // Assert
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void Gender_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Male";

        // Assert
        Assert.Equal("Male", user.Gender);
    }

    [Fact]
    public void PasswordHash_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "hashedPassword123";

        // Assert
        Assert.Equal("hashedPassword123", user.PasswordHash);
    }

    [Fact]
    public void DateOfBirth_CanBeSet()
    {
        // Arrange
        var user = new User();
        var dob = new DateTime(1990, 1, 1);

        // Act
        user.DateOfBirth = dob;

        // Assert
        Assert.Equal(dob, user.DateOfBirth);
    }

    [Fact]
    public void Street_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.Street = "123 Main St";

        // Assert
        Assert.Equal("123 Main St", user.Street);
    }

    [Fact]
    public void City_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.City = "New York";

        // Assert
        Assert.Equal("New York", user.City);
    }

    [Fact]
    public void State_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.State = "NY";

        // Assert
        Assert.Equal("NY", user.State);
    }

    [Fact]
    public void CreatedDate_DefaultsToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var user = new User();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(user.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public void ModifiedDate_CanBeSet()
    {
        // Arrange
        var user = new User();
        var modifiedDate = DateTime.UtcNow;

        // Act
        user.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, user.ModifiedDate);
    }

    [Fact]
    public void IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void IsActive_CanBeSetToFalse()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal("System", user.CreatedBy);
    }

    [Fact]
    public void CreatedBy_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.CreatedBy = "Admin";

        // Assert
        Assert.Equal("Admin", user.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_CanBeSet()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = "Editor";

        // Assert
        Assert.Equal("Editor", user.ModifiedBy);
    }

    [Fact]
    public void Bookings_InitializesToEmptyList()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Bookings_CanAddBooking()
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
    public void AllProperties_CanBeSetSimultaneously()
    {
        // Arrange
        var user = new User();
        var now = DateTime.UtcNow;
        var dob = new DateTime(1985, 5, 15);

        // Act
        user.Id = 100;
        user.Email = "complete@test.com";
        user.FirstName = "Complete";
        user.LastName = "Test";
        user.Gender = "Female";
        user.PasswordHash = "hash123";
        user.DateOfBirth = dob;
        user.Street = "456 Elm St";
        user.City = "Boston";
        user.State = "MA";
        user.CreatedDate = now;
        user.ModifiedDate = now;
        user.IsActive = false;
        user.CreatedBy = "TestUser";
        user.ModifiedBy = "TestModifier";

        // Assert
        Assert.Equal(100, user.Id);
        Assert.Equal("complete@test.com", user.Email);
        Assert.Equal("Complete", user.FirstName);
        Assert.Equal("Test", user.LastName);
        Assert.Equal("Female", user.Gender);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(dob, user.DateOfBirth);
        Assert.Equal("456 Elm St", user.Street);
        Assert.Equal("Boston", user.City);
        Assert.Equal("MA", user.State);
        Assert.Equal(now, user.CreatedDate);
        Assert.Equal(now, user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.Equal("TestUser", user.CreatedBy);
        Assert.Equal("TestModifier", user.ModifiedBy);
    }

    [Fact]
    public void Email_CanBeEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void PasswordHash_CanBeEmpty()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.PasswordHash);
    }
}
