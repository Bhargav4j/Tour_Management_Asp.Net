using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_InitializesDefaultValues()
    {
        var user = new User();
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Null(user.LastName);
        Assert.Equal("System", user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.False(user.IsActive);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_SetProperties_ValuesAreSetCorrectly()
    {
        var user = new User();
        var createdDate = DateTime.UtcNow;
        user.Id = 1;
        user.Email = "test@example.com";
        user.PasswordHash = "hashedpassword123";
        user.FirstName = "John";
        user.LastName = "Doe";
        user.CreatedDate = createdDate;
        user.IsActive = true;

        Assert.Equal(1, user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hashedpassword123", user.PasswordHash);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_Email_CanBeSetAndRetrieved()
    {
        var user = new User();
        var expectedEmail = "user@test.com";
        user.Email = expectedEmail;
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void User_Bookings_InitializesToEmptyCollection()
    {
        var user = new User();
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Bookings_CanAddBooking()
    {
        var user = new User { Id = 1, Email = "test@test.com" };
        var booking = new Booking { Id = 1, UserId = 1, Email = "test@test.com" };
        user.Bookings.Add(booking);
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_LastName_CanBeNull()
    {
        var user = new User();
        user.LastName = null;
        Assert.Null(user.LastName);
    }
}
