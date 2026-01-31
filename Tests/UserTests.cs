using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Null(user.FullName);
        Assert.Null(user.Phone);
        Assert.Null(user.Address);
        Assert.False(user.IsAdmin);
        Assert.True(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Id_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedId = 123;

        // Act
        user.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, user.Id);
    }

    [Fact]
    public void Username_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedUsername = "john_doe";

        // Act
        user.Username = expectedUsername;

        // Assert
        Assert.Equal(expectedUsername, user.Username);
    }

    [Fact]
    public void Email_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedEmail = "john@example.com";

        // Act
        user.Email = expectedEmail;

        // Assert
        Assert.Equal(expectedEmail, user.Email);
    }

    [Fact]
    public void PasswordHash_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedHash = "hashed_password_123";

        // Act
        user.PasswordHash = expectedHash;

        // Assert
        Assert.Equal(expectedHash, user.PasswordHash);
    }

    [Fact]
    public void FullName_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedFullName = "John Doe";

        // Act
        user.FullName = expectedFullName;

        // Assert
        Assert.Equal(expectedFullName, user.FullName);
    }

    [Fact]
    public void FullName_SetNull_ShouldWork()
    {
        // Arrange
        var user = new User { FullName = "John Doe" };

        // Act
        user.FullName = null;

        // Assert
        Assert.Null(user.FullName);
    }

    [Fact]
    public void Phone_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedPhone = "123-456-7890";

        // Act
        user.Phone = expectedPhone;

        // Assert
        Assert.Equal(expectedPhone, user.Phone);
    }

    [Fact]
    public void Phone_SetNull_ShouldWork()
    {
        // Arrange
        var user = new User { Phone = "123-456-7890" };

        // Act
        user.Phone = null;

        // Assert
        Assert.Null(user.Phone);
    }

    [Fact]
    public void Address_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedAddress = "123 Main St, City";

        // Act
        user.Address = expectedAddress;

        // Assert
        Assert.Equal(expectedAddress, user.Address);
    }

    [Fact]
    public void Address_SetNull_ShouldWork()
    {
        // Arrange
        var user = new User { Address = "123 Main St" };

        // Act
        user.Address = null;

        // Assert
        Assert.Null(user.Address);
    }

    [Fact]
    public void IsAdmin_SetToTrue_ShouldWork()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsAdmin = true;

        // Assert
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.UtcNow.AddDays(-1);

        // Act
        user.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedDate = DateTime.UtcNow;

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Fact]
    public void IsActive_SetToFalse_ShouldWork()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void CreatedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedCreatedBy = "Admin";

        // Act
        user.CreatedBy = expectedCreatedBy;

        // Assert
        Assert.Equal(expectedCreatedBy, user.CreatedBy);
    }

    [Fact]
    public void ModifiedBy_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var expectedModifiedBy = "Admin";

        // Act
        user.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedModifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void Bookings_SetAndGet_ShouldWork()
    {
        // Arrange
        var user = new User();
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1 },
            new Booking { Id = 2, UserId = 1 }
        };

        // Act
        user.Bookings = bookings;

        // Assert
        Assert.Equal(2, user.Bookings.Count);
        Assert.Contains(user.Bookings, b => b.Id == 1);
        Assert.Contains(user.Bookings, b => b.Id == 2);
    }

    [Fact]
    public void Username_WithEmptyString_ShouldSet()
    {
        // Arrange
        var user = new User { Username = "john_doe" };

        // Act
        user.Username = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Username);
    }

    [Fact]
    public void Email_WithEmptyString_ShouldSet()
    {
        // Arrange
        var user = new User { Email = "john@example.com" };

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void PasswordHash_WithEmptyString_ShouldSet()
    {
        // Arrange
        var user = new User { PasswordHash = "hash123" };

        // Act
        user.PasswordHash = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.PasswordHash);
    }
}
