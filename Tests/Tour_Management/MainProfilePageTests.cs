using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class MainProfilePageTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var mainProfilePage = new MainProfilePage();

            // Assert
            Assert.NotNull(mainProfilePage);
        }

        [Fact]
        public void MainProfilePage_InheritsFrom_PageClass()
        {
            // Arrange
            var mainProfilePage = new MainProfilePage();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(mainProfilePage);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var mainProfilePage = new MainProfilePage();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => mainProfilePage.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var mainProfilePage = new MainProfilePage();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => mainProfilePage.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var mainProfilePage = new MainProfilePage();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => mainProfilePage.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
