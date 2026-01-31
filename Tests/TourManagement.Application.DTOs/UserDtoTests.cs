using System;
using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests
{
    /// <summary>
    /// Tests for UserDto
    /// </summary>
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new UserDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Email);
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new UserDto();
            var createdDate = DateTime.Now;

            // Act
            dto.Id = 50;
            dto.Email = "user@test.com";
            dto.FullName = "Test User";
            dto.Phone = "123-456-7890";
            dto.Address = "123 Main St";
            dto.CreatedDate = createdDate;
            dto.IsActive = true;

            // Assert
            Assert.Equal(50, dto.Id);
            Assert.Equal("user@test.com", dto.Email);
            Assert.Equal("Test User", dto.FullName);
            Assert.Equal("123-456-7890", dto.Phone);
            Assert.Equal("123 Main St", dto.Address);
            Assert.Equal(createdDate, dto.CreatedDate);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void UserDto_NullableProperties_CanBeNull()
        {
            // Arrange
            var dto = new UserDto();

            // Act
            dto.FullName = null;
            dto.Phone = null;
            dto.Address = null;

            // Assert
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserDto_IsActive_ShouldSetCorrectly(bool isActive)
        {
            // Arrange
            var dto = new UserDto();

            // Act
            dto.IsActive = isActive;

            // Assert
            Assert.Equal(isActive, dto.IsActive);
        }
    }

    /// <summary>
    /// Tests for UserCreateDto
    /// </summary>
    public class UserCreateDtoTests
    {
        [Fact]
        public void UserCreateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new UserCreateDto();

            // Assert
            Assert.Equal(string.Empty, dto.Email);
            Assert.Equal(string.Empty, dto.Password);
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
        }

        [Fact]
        public void UserCreateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new UserCreateDto();

            // Act
            dto.Email = "newuser@example.com";
            dto.Password = "SecurePassword123";
            dto.FullName = "John Doe";
            dto.Phone = "555-1234";
            dto.Address = "456 Oak Avenue";

            // Assert
            Assert.Equal("newuser@example.com", dto.Email);
            Assert.Equal("SecurePassword123", dto.Password);
            Assert.Equal("John Doe", dto.FullName);
            Assert.Equal("555-1234", dto.Phone);
            Assert.Equal("456 Oak Avenue", dto.Address);
        }

        [Fact]
        public void UserCreateDto_NullableProperties_CanBeNull()
        {
            // Arrange
            var dto = new UserCreateDto();

            // Act
            dto.FullName = null;
            dto.Phone = null;
            dto.Address = null;

            // Assert
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
        }

        [Fact]
        public void UserCreateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var dto = new UserCreateDto
            {
                Email = "test@email.com",
                Password = "Password456",
                FullName = "Jane Smith",
                Phone = "555-9999",
                Address = "789 Pine Road"
            };

            // Assert
            Assert.Equal("test@email.com", dto.Email);
            Assert.Equal("Password456", dto.Password);
            Assert.Equal("Jane Smith", dto.FullName);
            Assert.Equal("555-9999", dto.Phone);
            Assert.Equal("789 Pine Road", dto.Address);
        }

        [Theory]
        [InlineData("user1@example.com")]
        [InlineData("admin@domain.org")]
        [InlineData("")]
        public void UserCreateDto_Email_ShouldAcceptVariousValues(string email)
        {
            // Arrange
            var dto = new UserCreateDto();

            // Act
            dto.Email = email;

            // Assert
            Assert.Equal(email, dto.Email);
        }

        [Theory]
        [InlineData("password123")]
        [InlineData("")]
        [InlineData("VeryLongPasswordWithSpecialCharacters!@#$%")]
        public void UserCreateDto_Password_ShouldAcceptVariousValues(string password)
        {
            // Arrange
            var dto = new UserCreateDto();

            // Act
            dto.Password = password;

            // Assert
            Assert.Equal(password, dto.Password);
        }
    }

    /// <summary>
    /// Tests for UserUpdateDto
    /// </summary>
    public class UserUpdateDtoTests
    {
        [Fact]
        public void UserUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new UserUpdateDto();

            // Assert
            Assert.Equal(string.Empty, dto.Email);
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserUpdateDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new UserUpdateDto();

            // Act
            dto.Email = "updated@example.com";
            dto.FullName = "Updated Name";
            dto.Phone = "555-7777";
            dto.Address = "999 Updated St";
            dto.IsActive = true;

            // Assert
            Assert.Equal("updated@example.com", dto.Email);
            Assert.Equal("Updated Name", dto.FullName);
            Assert.Equal("555-7777", dto.Phone);
            Assert.Equal("999 Updated St", dto.Address);
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void UserUpdateDto_NullableProperties_CanBeNull()
        {
            // Arrange
            var dto = new UserUpdateDto();

            // Act
            dto.FullName = null;
            dto.Phone = null;
            dto.Address = null;

            // Assert
            Assert.Null(dto.FullName);
            Assert.Null(dto.Phone);
            Assert.Null(dto.Address);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void UserUpdateDto_IsActive_ShouldSetCorrectly(bool isActive)
        {
            // Arrange
            var dto = new UserUpdateDto();

            // Act
            dto.IsActive = isActive;

            // Assert
            Assert.Equal(isActive, dto.IsActive);
        }

        [Fact]
        public void UserUpdateDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var dto = new UserUpdateDto
            {
                Email = "objectinit@example.com",
                FullName = "Object Init User",
                Phone = "555-0000",
                Address = "111 Object St",
                IsActive = false
            };

            // Assert
            Assert.Equal("objectinit@example.com", dto.Email);
            Assert.Equal("Object Init User", dto.FullName);
            Assert.Equal("555-0000", dto.Phone);
            Assert.Equal("111 Object St", dto.Address);
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserUpdateDto_InactiveUser_ShouldHaveIsActiveFalse()
        {
            // Arrange
            var dto = new UserUpdateDto();

            // Act
            dto.IsActive = false;

            // Assert
            Assert.False(dto.IsActive);
        }
    }
}
