using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(0, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.Equal(string.Empty, user.CreatedBy);
        Assert.Null(user.ModifiedBy);
        Assert.Null(user.ModifiedDate);
        Assert.False(user.IsActive);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_SetId_ShouldUpdateIdProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 123;

        // Assert
        Assert.Equal(123, user.Id);
    }

    [Fact]
    public void User_SetEmail_ShouldUpdateEmailProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "test@example.com";

        // Assert
        Assert.Equal("test@example.com", user.Email);
    }

    [Fact]
    public void User_SetFirstName_ShouldUpdateFirstNameProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.FirstName = "John";

        // Assert
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public void User_SetLastName_ShouldUpdateLastNameProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.LastName = "Doe";

        // Assert
        Assert.Equal("Doe", user.LastName);
    }

    [Fact]
    public void User_SetGender_ShouldUpdateGenderProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Gender = "Male";

        // Assert
        Assert.Equal("Male", user.Gender);
    }

    [Fact]
    public void User_SetPasswordHash_ShouldUpdatePasswordHashProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.PasswordHash = "hashedpassword123";

        // Assert
        Assert.Equal("hashedpassword123", user.PasswordHash);
    }

    [Fact]
    public void User_SetDateOfBirth_ShouldUpdateDateOfBirthProperty()
    {
        // Arrange
        var user = new User();
        var dob = new DateTime(1990, 5, 15);

        // Act
        user.DateOfBirth = dob;

        // Assert
        Assert.Equal(dob, user.DateOfBirth);
    }

    [Fact]
    public void User_SetStreet_ShouldUpdateStreetProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.Street = "123 Main St";

        // Assert
        Assert.Equal("123 Main St", user.Street);
    }

    [Fact]
    public void User_SetCity_ShouldUpdateCityProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.City = "New York";

        // Assert
        Assert.Equal("New York", user.City);
    }

    [Fact]
    public void User_SetState_ShouldUpdateStateProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.State = "NY";

        // Assert
        Assert.Equal("NY", user.State);
    }

    [Fact]
    public void User_SetCreatedDate_ShouldUpdateCreatedDateProperty()
    {
        // Arrange
        var user = new User();
        var createdDate = DateTime.UtcNow;

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
        var modifiedDate = DateTime.UtcNow;

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

        // Act
        user.CreatedBy = "admin";

        // Assert
        Assert.Equal("admin", user.CreatedBy);
    }

    [Fact]
    public void User_SetModifiedBy_ShouldUpdateModifiedByProperty()
    {
        // Arrange
        var user = new User();

        // Act
        user.ModifiedBy = "admin";

        // Assert
        Assert.Equal("admin", user.ModifiedBy);
    }

    [Fact]
    public void User_SetBookings_ShouldUpdateBookingsCollection()
    {
        // Arrange
        var user = new User();
        var booking = new Booking { Id = 1 };

        // Act
        user.Bookings = new List<Booking> { booking };

        // Assert
        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_AddBooking_ShouldAddToBookingsCollection()
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

    [Fact]
    public void User_WithCompleteData_ShouldRetainAllProperties()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "hashedpassword",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main Street",
            City = "New York",
            State = "NY",
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "system",
            ModifiedBy = "admin"
        };

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashedpassword", user.PasswordHash);
        Assert.Equal(new DateTime(1990, 1, 1), user.DateOfBirth);
        Assert.Equal("123 Main Street", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.NotNull(user.CreatedDate);
        Assert.NotNull(user.ModifiedDate);
        Assert.True(user.IsActive);
        Assert.Equal("system", user.CreatedBy);
        Assert.Equal("admin", user.ModifiedBy);
    }

    [Fact]
    public void User_SetEmptyEmail_ShouldAcceptEmptyString()
    {
        // Arrange
        var user = new User();

        // Act
        user.Email = "";

        // Assert
        Assert.Equal(string.Empty, user.Email);
    }

    [Fact]
    public void User_SetNullModifiedBy_ShouldAcceptNull()
    {
        // Arrange
        var user = new User { ModifiedBy = "admin" };

        // Act
        user.ModifiedBy = null;

        // Assert
        Assert.Null(user.ModifiedBy);
    }

    [Fact]
    public void User_SetNullModifiedDate_ShouldAcceptNull()
    {
        // Arrange
        var user = new User { ModifiedDate = DateTime.UtcNow };

        // Act
        user.ModifiedDate = null;

        // Assert
        Assert.Null(user.ModifiedDate);
    }
}
