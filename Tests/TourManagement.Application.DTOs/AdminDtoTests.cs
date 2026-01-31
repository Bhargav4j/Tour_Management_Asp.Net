using System;
using Xunit;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests
{
    /// <summary>
    /// Tests for AdminDto
    /// </summary>
    public class AdminDtoTests
    {
        [Fact]
        public void AdminDto_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var dto = new AdminDto();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Username);
            Assert.Equal(string.Empty, dto.Email);
        }

        [Fact]
        public void AdminDto_Id_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new AdminDto();
            var expectedId = 42;

            // Act
            dto.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, dto.Id);
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("superuser")]
        [InlineData("moderator")]
        public void AdminDto_Username_ShouldSetAndGetCorrectly(string expectedUsername)
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Username = expectedUsername;

            // Assert
            Assert.Equal(expectedUsername, dto.Username);
        }

        [Theory]
        [InlineData("admin@example.com")]
        [InlineData("super@domain.org")]
        [InlineData("")]
        public void AdminDto_Email_ShouldSetAndGetCorrectly(string expectedEmail)
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Email = expectedEmail;

            // Assert
            Assert.Equal(expectedEmail, dto.Email);
        }

        [Fact]
        public void AdminDto_AllProperties_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Id = 100;
            dto.Username = "testadmin";
            dto.Email = "testadmin@test.com";

            // Assert
            Assert.Equal(100, dto.Id);
            Assert.Equal("testadmin", dto.Username);
            Assert.Equal("testadmin@test.com", dto.Email);
        }

        [Fact]
        public void AdminDto_WithObjectInitializer_ShouldSetAllProperties()
        {
            // Arrange & Act
            var dto = new AdminDto
            {
                Id = 200,
                Username = "objectadmin",
                Email = "object@admin.com"
            };

            // Assert
            Assert.Equal(200, dto.Id);
            Assert.Equal("objectadmin", dto.Username);
            Assert.Equal("object@admin.com", dto.Email);
        }

        [Fact]
        public void AdminDto_WithEmptyUsername_ShouldBeValid()
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Username = string.Empty;

            // Assert
            Assert.Equal(string.Empty, dto.Username);
        }

        [Fact]
        public void AdminDto_WithEmptyEmail_ShouldBeValid()
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Email = string.Empty;

            // Assert
            Assert.Equal(string.Empty, dto.Email);
        }

        [Fact]
        public void AdminDto_WithZeroId_ShouldBeValid()
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Id = 0;

            // Assert
            Assert.Equal(0, dto.Id);
        }

        [Fact]
        public void AdminDto_WithNegativeId_ShouldBeValid()
        {
            // Arrange
            var dto = new AdminDto();

            // Act
            dto.Id = -1;

            // Assert
            Assert.Equal(-1, dto.Id);
        }

        [Fact]
        public void AdminDto_WithLongUsername_ShouldSetCorrectly()
        {
            // Arrange
            var dto = new AdminDto();
            var longUsername = new string('a', 100);

            // Act
            dto.Username = longUsername;

            // Assert
            Assert.Equal(longUsername, dto.Username);
        }

        [Fact]
        public void AdminDto_WithLongEmail_ShouldSetCorrectly()
        {
            // Arrange
            var dto = new AdminDto();
            var longEmail = new string('b', 50) + "@example.com";

            // Act
            dto.Email = longEmail;

            // Assert
            Assert.Equal(longEmail, dto.Email);
        }
    }
}
