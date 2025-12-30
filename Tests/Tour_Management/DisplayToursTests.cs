using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class DisplayToursTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var displayTours = new DisplayTours();

            // Assert
            Assert.NotNull(displayTours);
        }

        [Fact]
        public void DisplayTours_InheritsFrom_PageClass()
        {
            // Arrange
            var displayTours = new DisplayTours();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(displayTours);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var displayTours = new DisplayTours();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => displayTours.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var displayTours = new DisplayTours();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => displayTours.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var displayTours = new DisplayTours();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => displayTours.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }
    }
}
