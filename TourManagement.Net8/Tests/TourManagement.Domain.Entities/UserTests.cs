using Xunit;
using TourManagement.Domain.Entities;
using System;
using System.Linq;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_CreatesUser_WithDefaultValues()
    {
        // Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal("System", user.CreatedBy);
        Assert.NotNull(user.Bookings);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var user = new User();
        var dob = new DateTime(1990, 1, 1);

        // Act
        user.Id = 1;
        user.Email = "test@test.com";
        user.FirstName = "John";
        user.LastName = "Doe";
        user.Gender = "Male";
        user.PasswordHash = "hash123";
        user.DateOfBirth = dob;
        user.Street = "123 Main St";
        user.City = "New York";
        user.State = "NY";
        user.IsActive = true;

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("test@test.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hash123", user.PasswordHash);
        Assert.Equal(dob, user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Bookings_CanAddBookings()
    {
        // Arrange
        var user = new User();
        var booking1 = new Booking { Id = 1, NumberOfPersons = 2 };
        var booking2 = new Booking { Id = 2, NumberOfPersons = 3 };

        // Act
        user.Bookings.Add(booking1);
        user.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, user.Bookings.Count);
        Assert.Contains(booking1, user.Bookings);
        Assert.Contains(booking2, user.Bookings);
    }
}
