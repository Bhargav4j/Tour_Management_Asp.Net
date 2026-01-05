using Xunit;
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
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(default(DateTime), user.DateOfBirth);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.Equal(default(DateTime), user.CreatedDate);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_Email_ShouldSetAndGetCorrectly()
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
    public void User_Email_WithEmptyString_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };

        // Act
        user.Email = string.Empty;

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void User_FirstName_ShouldSetAndGetCorrectly()
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
    public void User_LastName_ShouldSetAndGetCorrectly()
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
    public void User_Gender_ShouldSetAndGetCorrectly()
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
    public void User_Gender_WithFemale_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Female";

        // Assert
        Assert.Equal("Female", user.Gender);
    }

    [Fact]
    public void User_PasswordHash_ShouldSetAndGetCorrectly()
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
    public void User_DateOfBirth_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(1990, 5, 15);

        // Act
        user.DateOfBirth = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.DateOfBirth);
    }

    [Fact]
    public void User_Street_ShouldSetAndGetCorrectly()
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
    public void User_City_ShouldSetAndGetCorrectly()
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
    public void User_State_ShouldSetAndGetCorrectly()
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
    public void User_CreatedDate_ShouldSetAndGetCorrectly()
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
    public void User_ModifiedDate_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        user.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, user.ModifiedDate);
    }

    [Fact]
    public void User_ModifiedDate_WithNull_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User { ModifiedDate = DateTime.Now };

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_IsActive_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = true;

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_IsActive_WithFalse_ShouldSetCorrectly()
    {
        // Arrange
        var user = new User { IsActive = true };

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_Bookings_ShouldInitializeAsEmptyCollection()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(user.Bookings);
    }

    [Fact]
    public void User_Bookings_ShouldAddBookingCorrectly()
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
    public void User_Bookings_ShouldAddMultipleBookingsCorrectly()
    {
        // Arrange
        var user = new User();
        var booking1 = new Booking();
        var booking2 = new Booking();
        var booking3 = new Booking();

        // Act
        user.Bookings.Add(booking1);
        user.Bookings.Add(booking2);
        user.Bookings.Add(booking3);

        // Assert
        Assert.Equal(3, user.Bookings.Count);
        Assert.Contains(booking1, user.Bookings);
        Assert.Contains(booking2, user.Bookings);
        Assert.Contains(booking3, user.Bookings);
    }

    [Fact]
    public void User_AllProperties_ShouldSetAndGetCorrectly()
    {
        // Arrange
        var user = new User();
        var expectedEmail = "john.doe@example.com";
        var expectedFirstName = "John";
        var expectedLastName = "Doe";
        var expectedGender = "Male";
        var expectedPasswordHash = "$2a$11$hashedpassword";
        var expectedDateOfBirth = new DateTime(1990, 5, 15);
        var expectedStreet = "123 Main St";
        var expectedCity = "New York";
        var expectedState = "NY";
        var expectedCreatedDate = new DateTime(2024, 1, 1);
        var expectedModifiedDate = new DateTime(2024, 1, 2);
        var expectedIsActive = true;

        // Act
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

        // Assert
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
    }

    [Fact]
    public void User_FullName_Combination_ShouldBeCorrect()
    {
        // Arrange
        var user = new User
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        // Act
        var fullName = $"{user.FirstName} {user.LastName}";

        // Assert
        Assert.Equal("Jane Smith", fullName);
    }

    [Fact]
    public void User_Address_Combination_ShouldBeCorrect()
    {
        // Arrange
        var user = new User
        {
            Street = "456 Elm St",
            City = "Boston",
            State = "MA"
        };

        // Act
        var fullAddress = $"{user.Street}, {user.City}, {user.State}";

        // Assert
        Assert.Equal("456 Elm St, Boston, MA", fullAddress);
    }
}
