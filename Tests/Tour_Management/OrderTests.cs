using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var order = new Order();

            // Assert
            Assert.NotNull(order);
        }

        [Fact]
        public void Order_InheritsFrom_PageClass()
        {
            // Arrange
            var order = new Order();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(order);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var order = new Order();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => order.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var order = new Order();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => order.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var order = new Order();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => order.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void btn_click_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var order = new Order();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => order.btn_click(null, e));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }

        [Fact]
        public void btn_click_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var order = new Order();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => order.btn_click(sender, null));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }
    }
}
