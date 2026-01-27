using System;
using System.Web.UI;
using Xunit;

namespace SystemWebUI.Tests
{
    public class PageTests
    {
        [Fact]
        public void Page_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new Page();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void Page_Request_ShouldNotBeNull()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.NotNull(page.Request);
        }

        [Fact]
        public void Page_Response_ShouldNotBeNull()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.NotNull(page.Response);
        }

        [Fact]
        public void Page_Server_ShouldNotBeNull()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.NotNull(page.Server);
        }

        [Fact]
        public void Page_Session_ShouldNotBeNull()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.NotNull(page.Session);
        }

        [Fact]
        public void Page_Application_ShouldNotBeNull()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.NotNull(page.Application);
        }

        [Fact]
        public void Page_IsPostBack_ShouldBeSettable()
        {
            // Arrange
            var page = new Page();

            // Act
            page.IsPostBack = true;

            // Assert
            Assert.True(page.IsPostBack);
        }

        [Fact]
        public void Page_IsPostBack_DefaultValue_ShouldBeFalse()
        {
            // Arrange
            var page = new Page();

            // Act & Assert
            Assert.False(page.IsPostBack);
        }
    }
}
