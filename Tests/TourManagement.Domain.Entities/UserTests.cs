using System;
using System.Collections.Generic;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests
{
    /// <summary>
    /// Tests for User entity
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
            Assert.Null(user.FullName);
            Assert.Null(user.Phone);
            Assert.Null(user.Address);
            Assert.True(user.IsActive);
            Assert.Equal("System", user.CreatedBy);
            Assert.Null(user.ModifiedBy);
            Assert.NotNull(user.Bookings);
            Assert.Empty(user.Bookings);
        }

        [Fact]
        public void User_Id_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedId = 456;

            // Act
            user.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, user.Id);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user@domain.org")]
        [InlineData("")]
        public void User_Email_ShouldSetAndGetCorrectly(string expectedEmail)
        {
            // Arrange
            var user = new User();

            // Act
            user.Email = expectedEmail;

            // Assert
            Assert.Equal(expectedEmail, user.Email);
        }

        [Fact]
        public void User_PasswordHash_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedHash = "hashedpassword123";

            // Act
            user.PasswordHash = expectedHash;

            // Assert
            Assert.Equal(expectedHash, user.PasswordHash);
        }

        [Fact]
        public void User_FullName_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedName = "John Doe";

            // Act
            user.FullName = expectedName;

            // Assert
            Assert.Equal(expectedName, user.FullName);
        }

        [Fact]
        public void User_FullName_CanBeNull()
        {
            // Arrange
            var user = new User();

            // Act
            user.FullName = null;

            // Assert
            Assert.Null(user.FullName);
        }

        [Theory]
        [InlineData("123-456-7890")]
        [InlineData("+1-555-1234")]
        [InlineData(null)]
        public void User_Phone_ShouldSetAndGetCorrectly(string? expectedPhone)
        {
            // Arrange
            var user = new User();

            // Act
            user.Phone = expectedPhone;

            // Assert
            Assert.Equal(expectedPhone, user.Phone);
        }

        [Fact]
        public void User_Address_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedAddress = "123 Main St, City, State";

            // Act
            user.Address = expectedAddress;

            // Assert
            Assert.Equal(expectedAddress, user.Address);
        }

        [Fact]
        public void User_Address_CanBeNull()
        {
            // Arrange
            var user = new User();

            // Act
            user.Address = null;

            // Assert
            Assert.Null(user.Address);
        }

        [Fact]
        public void User_CreatedDate_ShouldSetAndGetCorrectly()
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
        public void User_ModifiedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedDate = DateTime.Now.AddDays(5);

            // Act
            user.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, user.ModifiedDate);
        }

        [Fact]
        public void User_ModifiedDate_CanBeNull()
        {
            // Arrange
            var user = new User();

            // Act
            user.ModifiedDate = null;

            // Assert
            Assert.Null(user.ModifiedDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void User_IsActive_ShouldSetAndGetCorrectly(bool expectedValue)
        {
            // Arrange
            var user = new User();

            // Act
            user.IsActive = expectedValue;

            // Assert
            Assert.Equal(expectedValue, user.IsActive);
        }

        [Fact]
        public void User_CreatedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedCreator = "AdminUser";

            // Act
            user.CreatedBy = expectedCreator;

            // Assert
            Assert.Equal(expectedCreator, user.CreatedBy);
        }

        [Fact]
        public void User_ModifiedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var user = new User();
            var expectedModifier = "Editor123";

            // Act
            user.ModifiedBy = expectedModifier;

            // Assert
            Assert.Equal(expectedModifier, user.ModifiedBy);
        }

        [Fact]
        public void User_ModifiedBy_CanBeNull()
        {
            // Arrange
            var user = new User();

            // Act
            user.ModifiedBy = null;

            // Assert
            Assert.Null(user.ModifiedBy);
        }

        [Fact]
        public void User_Bookings_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.NotNull(user.Bookings);
            Assert.Empty(user.Bookings);
        }

        [Fact]
        public void User_Bookings_ShouldAddBookingsCorrectly()
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
        public void User_FullObjectInitialization_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var createdDate = DateTime.Now;
            var modifiedDate = DateTime.Now.AddDays(2);
            var bookings = new List<Booking> { new Booking { Id = 10 } };

            // Act
            var user = new User
            {
                Id = 200,
                Email = "john@example.com",
                PasswordHash = "hash123abc",
                FullName = "John Smith",
                Phone = "555-1234",
                Address = "456 Elm Street",
                CreatedDate = createdDate,
                ModifiedDate = modifiedDate,
                IsActive = false,
                CreatedBy = "SystemAdmin",
                ModifiedBy = "UpdateUser",
                Bookings = bookings
            };

            // Assert
            Assert.Equal(200, user.Id);
            Assert.Equal("john@example.com", user.Email);
            Assert.Equal("hash123abc", user.PasswordHash);
            Assert.Equal("John Smith", user.FullName);
            Assert.Equal("555-1234", user.Phone);
            Assert.Equal("456 Elm Street", user.Address);
            Assert.Equal(createdDate, user.CreatedDate);
            Assert.Equal(modifiedDate, user.ModifiedDate);
            Assert.False(user.IsActive);
            Assert.Equal("SystemAdmin", user.CreatedBy);
            Assert.Equal("UpdateUser", user.ModifiedBy);
            Assert.Single(user.Bookings);
        }

        [Fact]
        public void User_WithEmptyEmail_ShouldBeValid()
        {
            // Arrange
            var user = new User();

            // Act
            user.Email = string.Empty;

            // Assert
            Assert.Equal(string.Empty, user.Email);
        }

        [Fact]
        public void User_WithEmptyPasswordHash_ShouldBeValid()
        {
            // Arrange
            var user = new User();

            // Act
            user.PasswordHash = string.Empty;

            // Assert
            Assert.Equal(string.Empty, user.PasswordHash);
        }
    }
}
