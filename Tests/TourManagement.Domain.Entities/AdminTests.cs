using System;
using Xunit;
using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Entities.Tests
{
    /// <summary>
    /// Tests for Admin entity
    /// </summary>
    public class AdminTests
    {
        [Fact]
        public void Admin_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var admin = new Admin();

            // Assert
            Assert.Equal(0, admin.Id);
            Assert.Equal(string.Empty, admin.Username);
            Assert.Equal(string.Empty, admin.PasswordHash);
            Assert.Equal(string.Empty, admin.Email);
            Assert.True(admin.IsActive);
            Assert.Equal("System", admin.CreatedBy);
            Assert.Null(admin.ModifiedBy);
        }

        [Fact]
        public void Admin_Id_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedId = 999;

            // Act
            admin.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, admin.Id);
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("superuser")]
        [InlineData("")]
        public void Admin_Username_ShouldSetAndGetCorrectly(string expectedUsername)
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.Username = expectedUsername;

            // Assert
            Assert.Equal(expectedUsername, admin.Username);
        }

        [Fact]
        public void Admin_PasswordHash_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedHash = "hashedAdminPassword456";

            // Act
            admin.PasswordHash = expectedHash;

            // Assert
            Assert.Equal(expectedHash, admin.PasswordHash);
        }

        [Theory]
        [InlineData("admin@example.com")]
        [InlineData("superadmin@domain.org")]
        [InlineData("")]
        public void Admin_Email_ShouldSetAndGetCorrectly(string expectedEmail)
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.Email = expectedEmail;

            // Assert
            Assert.Equal(expectedEmail, admin.Email);
        }

        [Fact]
        public void Admin_CreatedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedDate = DateTime.Now;

            // Act
            admin.CreatedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, admin.CreatedDate);
        }

        [Fact]
        public void Admin_ModifiedDate_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedDate = DateTime.Now.AddDays(7);

            // Act
            admin.ModifiedDate = expectedDate;

            // Assert
            Assert.Equal(expectedDate, admin.ModifiedDate);
        }

        [Fact]
        public void Admin_ModifiedDate_CanBeNull()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.ModifiedDate = null;

            // Assert
            Assert.Null(admin.ModifiedDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Admin_IsActive_ShouldSetAndGetCorrectly(bool expectedValue)
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.IsActive = expectedValue;

            // Assert
            Assert.Equal(expectedValue, admin.IsActive);
        }

        [Fact]
        public void Admin_CreatedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedCreator = "RootAdmin";

            // Act
            admin.CreatedBy = expectedCreator;

            // Assert
            Assert.Equal(expectedCreator, admin.CreatedBy);
        }

        [Fact]
        public void Admin_ModifiedBy_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var admin = new Admin();
            var expectedModifier = "SuperUser789";

            // Act
            admin.ModifiedBy = expectedModifier;

            // Assert
            Assert.Equal(expectedModifier, admin.ModifiedBy);
        }

        [Fact]
        public void Admin_ModifiedBy_CanBeNull()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.ModifiedBy = null;

            // Assert
            Assert.Null(admin.ModifiedBy);
        }

        [Fact]
        public void Admin_FullObjectInitialization_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var createdDate = DateTime.Now;
            var modifiedDate = DateTime.Now.AddDays(10);

            // Act
            var admin = new Admin
            {
                Id = 300,
                Username = "superadmin",
                PasswordHash = "secureHashedPassword",
                Email = "admin@tourmanagement.com",
                CreatedDate = createdDate,
                ModifiedDate = modifiedDate,
                IsActive = true,
                CreatedBy = "RootSystem",
                ModifiedBy = "MasterAdmin"
            };

            // Assert
            Assert.Equal(300, admin.Id);
            Assert.Equal("superadmin", admin.Username);
            Assert.Equal("secureHashedPassword", admin.PasswordHash);
            Assert.Equal("admin@tourmanagement.com", admin.Email);
            Assert.Equal(createdDate, admin.CreatedDate);
            Assert.Equal(modifiedDate, admin.ModifiedDate);
            Assert.True(admin.IsActive);
            Assert.Equal("RootSystem", admin.CreatedBy);
            Assert.Equal("MasterAdmin", admin.ModifiedBy);
        }

        [Fact]
        public void Admin_WithEmptyUsername_ShouldBeValid()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.Username = string.Empty;

            // Assert
            Assert.Equal(string.Empty, admin.Username);
        }

        [Fact]
        public void Admin_WithEmptyPasswordHash_ShouldBeValid()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.PasswordHash = string.Empty;

            // Assert
            Assert.Equal(string.Empty, admin.PasswordHash);
        }

        [Fact]
        public void Admin_WithEmptyEmail_ShouldBeValid()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.Email = string.Empty;

            // Assert
            Assert.Equal(string.Empty, admin.Email);
        }

        [Fact]
        public void Admin_InactiveAdmin_ShouldHaveIsActiveFalse()
        {
            // Arrange
            var admin = new Admin();

            // Act
            admin.IsActive = false;

            // Assert
            Assert.False(admin.IsActive);
        }
    }
}
