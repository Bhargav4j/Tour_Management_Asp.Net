using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class mybookingTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var mybooking = new mybooking();

            // Assert
            Assert.NotNull(mybooking);
        }

        [Fact]
        public void mybooking_InheritsFrom_PageClass()
        {
            // Arrange
            var mybooking = new mybooking();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(mybooking);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var mybooking = new mybooking();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => mybooking.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var mybooking = new mybooking();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => mybooking.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var mybooking = new mybooking();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => mybooking.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
