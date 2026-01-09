using TourManagement.Domain.Entities;
using Xunit;

namespace TourManagement.UnitTests.Entities;

/// <summary>
/// Unit tests for User entity
/// </summary>
public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Null(user.PhoneNumber);
        Assert.Null(user.Address);
        Assert.False(user.IsAdmin);
        Assert.Equal(default(DateTime), user.CreatedDate);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.Equal("System", user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 1;

        // Assert
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public void User_SetEmail_ShouldUpdateEmailProperty()
    {
        // Arrange
        var user = new User();
        var email = "test@example.com";

        // Act
        user.Email = email;

        // Assert
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void User_SetPasswordHash_ShouldUpdatePasswordHashProperty()
    {
        // Arrange
        var user = new User();
        var passwordHash = "hashed_password_123";

        // Act
        user.PasswordHash = passwordHash;

        // Assert
        Assert.Equal(passwordHash, user.PasswordHash);
    }

    [Fact]
    public void User_SetFirstName_ShouldUpdateFirstNameProperty()
    {
        // Arrange
        var user = new User();
        var firstName = "John";

        // Act
        user.FirstName = firstName;

        // Assert
        Assert.Equal(firstName, user.FirstName);
    }

    [Fact]
    public void User_SetLastName_ShouldUpdateLastNameProperty()
    {
        // Arrange
        var user = new User();
        var lastName = "Doe";

        // Act
        user.LastName = lastName;

        // Assert
        Assert.Equal(lastName, user.LastName);
    }

    [Fact]
    public void User_SetPhoneNumber_ShouldUpdatePhoneNumberProperty()
    {
        // Arrange
        var user = new User();
        var phoneNumber = "123-456-7890";

        // Act
        user.PhoneNumber = phoneNumber;

        // Assert
        Assert.Equal(phoneNumber, user.PhoneNumber);
    }

    [Fact]
    public void User_SetAddress_ShouldUpdateAddressProperty()
    {
        // Arrange
        var user = new User();
        var address = "123 Main Street";

        // Act
        user.Address = address;

        // Assert
        Assert.Equal(address, user.Address);
    }

    [Fact]
    public void User_SetIsAdmin_ShouldUpdateIsAdminProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsAdmin = true;

        // Assert
        Assert.True(user.IsAdmin);
    }

    [Fact]
    public void User_SetCreatedDate_ShouldUpdateCreatedDateProperty()
    {
        // Arrange
        var user = new User();
        var createdDate = DateTime.Now;

        // Act
        user.CreatedDate = createdDate;

        // Assert
        Assert.Equal(createdDate, user.CreatedDate);
    }

    [Fact]
    public void User_SetModifiedDate_ShouldUpdateModifiedDateProperty()
    {
        // Arrange
        var user = new User();
        var modifiedDate = DateTime.Now;

        // Act
        user.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(modifiedDate, user.ModifiedDate);
    }

    [Fact]
    public void User_SetIsActive_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_SetCreatedBy_ShouldUpdateCreatedByProperty()
    {
        // Arrange
        var user = new User();
        var createdBy = "Admin";

        // Act
        user.CreatedBy = createdBy;

        // Assert
        Assert.Equal(createdBy, user.CreatedBy);
    }

    [Fact]
    public void User_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var user = new User();
        var modifiedBy = "Admin";

        // Act
        user.ModifiedBy = modifiedBy;

        // Assert
        Assert.Equal(modifiedBy, user.ModifiedBy);
    }

    [Fact]
    public void User_AddBooking_ShouldAddBookingToCollection()
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
    public void User_SetEmptyStrings_ShouldAcceptEmptyStrings()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = string.Empty;
        user.PasswordHash = string.Empty;
        user.FirstName = string.Empty;
        user.LastName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
    }

    [Fact]
    public void User_SetNullOptionalFields_ShouldAcceptNullValues()
    {
        // Arrange
        var user = new User();

        // Act
        user.PhoneNumber = null;
        user.Address = null;
        user.ModifiedBy = null;
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.PhoneNumber);
        Assert.Null(user.Address);
        Assert.Null(user.ModifiedBy);
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_MultipleBookings_ShouldManageCollection()
    {
        // Arrange
        var user = new User();
        var booking1 = new Booking { Id = 1 };
        var booking2 = new Booking { Id = 2 };

        // Act
        user.Bookings.Add(booking1);
        user.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, user.Bookings.Count);
        Assert.Contains(booking1, user.Bookings);
        Assert.Contains(booking2, user.Bookings);
    }
}
