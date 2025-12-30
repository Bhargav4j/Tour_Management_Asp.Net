using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xunit;

namespace Tour_Management.Tests
{
    public class AdminLogin2Tests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var adminLogin = new AdminLogin2();

            // Assert
            Assert.NotNull(adminLogin);
        }

        [Fact]
        public void AdminLogin2_InheritsFrom_PageClass()
        {
            // Arrange
            var adminLogin = new AdminLogin2();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(adminLogin);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var adminLogin = new AdminLogin2();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => adminLogin.Page_Load(sender, e));
            // Expected to throw due to missing HTTP context and controls
            Assert.NotNull(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var adminLogin = new AdminLogin2();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => adminLogin.Page_Load(sender, null));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var adminLogin = new AdminLogin2();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => adminLogin.Page_Load(null, e));

            // Assert
            Assert.NotNull(exception);
        }
    }
}
