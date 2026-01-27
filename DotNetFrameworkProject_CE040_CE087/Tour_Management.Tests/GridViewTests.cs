using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class GridViewTests
    {
        [Fact]
        public void GridView_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var gridView = new GridView();

            // Assert
            Assert.NotNull(gridView);
        }

        [Fact]
        public void GridView_DataSource_ShouldBeSettable()
        {
            // Arrange
            var gridView = new GridView();
            var dataSource = new[] { new { Id = 1, Name = "Test" } };

            // Act
            gridView.DataSource = dataSource;

            // Assert
            Assert.Equal(dataSource, gridView.DataSource);
        }

        [Fact]
        public void GridView_DataSource_DefaultValue_ShouldBeNull()
        {
            // Arrange
            var gridView = new GridView();

            // Act & Assert
            Assert.Null(gridView.DataSource);
        }

        [Fact]
        public void GridView_DataBind_ShouldNotThrow()
        {
            // Arrange
            var gridView = new GridView();

            // Act & Assert
            var exception = Record.Exception(() => gridView.DataBind());
            Assert.Null(exception);
        }

        [Fact]
        public void GridView_DataBind_WithDataSource_ShouldNotThrow()
        {
            // Arrange
            var gridView = new GridView();
            gridView.DataSource = new[] { new { Id = 1, Name = "Test" } };

            // Act & Assert
            var exception = Record.Exception(() => gridView.DataBind());
            Assert.Null(exception);
        }

        [Fact]
        public void GridView_ShouldInheritFromControl()
        {
            // Arrange
            var gridView = new GridView();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Control>(gridView);
        }
    }
}
