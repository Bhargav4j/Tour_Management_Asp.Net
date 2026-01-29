using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void User_Constructor_InitializesWithDefaultValues()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
        Assert.False(user.IsActive);
        Assert.NotNull(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_SetProperties_ShouldStoreValues()
    {
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            PasswordHash = "hashedpassword123",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY",
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("Male", user.Gender);
        Assert.Equal("hashedpassword123", user.PasswordHash);
        Assert.Equal(new DateTime(1990, 5, 15), user.DateOfBirth);
        Assert.Equal("123 Main St", user.Street);
        Assert.Equal("New York", user.City);
        Assert.Equal("NY", user.State);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_ModifiedDate_CanBeSetAndRetrieved()
    {
        var user = new User();
        var modifiedDate = DateTime.UtcNow;

        user.ModifiedDate = modifiedDate;

        Assert.Equal(modifiedDate, user.ModifiedDate);
    }

    [Fact]
    public void User_ModifiedDate_CanBeNull()
    {
        var user = new User { ModifiedDate = null };

        Assert.Null(user.ModifiedDate);
    }

    [Fact]
    public void User_Bookings_CanAddItems()
    {
        var user = new User { Email = "test@example.com" };
        var booking = new Booking { Id = 1, UserEmail = user.Email };

        user.Bookings.Add(booking);

        Assert.Single(user.Bookings);
        Assert.Contains(booking, user.Bookings);
    }

    [Fact]
    public void User_Gender_AcceptsValidValues()
    {
        var userMale = new User { Gender = "Male" };
        var userFemale = new User { Gender = "Female" };

        Assert.Equal("Male", userMale.Gender);
        Assert.Equal("Female", userFemale.Gender);
    }

    [Fact]
    public void User_IsActive_DefaultsToFalse()
    {
        var user = new User();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void User_IsActive_CanBeSetToTrue()
    {
        var user = new User { IsActive = true };

        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_DateOfBirth_CanBeSet()
    {
        var dateOfBirth = new DateTime(1985, 3, 20);
        var user = new User { DateOfBirth = dateOfBirth };

        Assert.Equal(dateOfBirth, user.DateOfBirth);
    }

    [Fact]
    public void User_CreatedDate_CanBeSet()
    {
        var createdDate = DateTime.UtcNow;
        var user = new User { CreatedDate = createdDate };

        Assert.Equal(createdDate, user.CreatedDate);
    }

    [Fact]
    public void User_Bookings_InitializesAsEmptyList()
    {
        var user = new User();

        Assert.NotNull(user.Bookings);
        Assert.IsAssignableFrom<ICollection<Booking>>(user.Bookings);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void User_AllStringProperties_InitializeAsEmpty()
    {
        var user = new User();

        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.FirstName);
        Assert.Equal(string.Empty, user.LastName);
        Assert.Equal(string.Empty, user.Gender);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(string.Empty, user.Street);
        Assert.Equal(string.Empty, user.City);
        Assert.Equal(string.Empty, user.State);
    }
}
