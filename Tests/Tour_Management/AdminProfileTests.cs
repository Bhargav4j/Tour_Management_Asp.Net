using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class AdminProfileTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var adminProfile = new AdminProfile();

            // Assert
            Assert.NotNull(adminProfile);
        }

        [Fact]
        public void AdminProfile_InheritsFrom_PageClass()
        {
            // Arrange
            var adminProfile = new AdminProfile();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(adminProfile);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var adminProfile = new AdminProfile();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => adminProfile.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var adminProfile = new AdminProfile();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => adminProfile.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var adminProfile = new AdminProfile();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => adminProfile.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
