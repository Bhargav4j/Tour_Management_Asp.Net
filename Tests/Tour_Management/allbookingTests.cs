using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class allbookingTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var allbooking = new allbooking();

            // Assert
            Assert.NotNull(allbooking);
        }

        [Fact]
        public void allbooking_InheritsFrom_PageClass()
        {
            // Arrange
            var allbooking = new allbooking();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(allbooking);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var allbooking = new allbooking();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => allbooking.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var allbooking = new allbooking();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => allbooking.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var allbooking = new allbooking();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => allbooking.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
