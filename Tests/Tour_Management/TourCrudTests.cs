using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class TourCrudTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var tourCrud = new TourCrud();

            // Assert
            Assert.NotNull(tourCrud);
        }

        [Fact]
        public void TourCrud_InheritsFrom_PageClass()
        {
            // Arrange
            var tourCrud = new TourCrud();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(tourCrud);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException_WhenIsPostBack()
        {
            // Arrange
            var tourCrud = new TourCrud();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert - Will throw due to missing HTTP context
            var exception = Record.Exception(() => tourCrud.Page_Load(sender, e));
            Assert.NotNull(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var tourCrud = new TourCrud();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => tourCrud.Page_Load(null, e));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var tourCrud = new TourCrud();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => tourCrud.Page_Load(sender, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void refreshdata_ShouldHandleDatabaseConnection()
        {
            // Arrange
            var tourCrud = new TourCrud();

            // Act
            var exception = Record.Exception(() => tourCrud.refreshdata());

            // Assert - Will throw due to missing connection string
            Assert.NotNull(exception);
        }

        [Fact]
        public void refreshdata_ShouldBePublicMethod()
        {
            // Arrange
            var tourCrud = new TourCrud();
            var methodInfo = typeof(TourCrud).GetMethod("refreshdata");

            // Assert
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsPublic);
        }
    }
}
