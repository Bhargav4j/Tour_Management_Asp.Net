using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class usercrudTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var usercrud = new usercrud();

            // Assert
            Assert.NotNull(usercrud);
        }

        [Fact]
        public void usercrud_InheritsFrom_PageClass()
        {
            // Arrange
            var usercrud = new usercrud();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(usercrud);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var usercrud = new usercrud();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => usercrud.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var usercrud = new usercrud();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => usercrud.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var usercrud = new usercrud();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => usercrud.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
