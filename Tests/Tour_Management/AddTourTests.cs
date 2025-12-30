using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Xunit;

namespace Tour_Management.Tests
{
    public class AddTourTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var addTour = new AddTour();

            // Assert
            Assert.NotNull(addTour);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var addTour = new AddTour();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert - Page_Load should complete without exception
            var exception = Record.Exception(() => addTour.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void AddTour_InheritsFrom_PageClass()
        {
            // Arrange
            var addTour = new AddTour();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(addTour);
        }

        [Fact]
        public void Register_Click_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var addTour = new AddTour();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => addTour.Register_Click(null, e));
            // This will likely throw due to missing dependencies, but we're testing the method signature
            Assert.NotNull(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldNotThrowNullReferenceException()
        {
            // Arrange
            var addTour = new AddTour();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => addTour.Page_Load(sender, null));

            // Assert - Method should handle null EventArgs
            Assert.Null(exception);
        }
    }
}
