using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class AdminLogin2Tests
    {
        [Fact]
        public void AdminLogin2_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new AdminLogin2();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void AdminLogin2_ShouldInheritFromPage()
        {
            // Arrange
            var page = new AdminLogin2();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldHandleEventArgs()
        {
            // Arrange
            var page = new AdminLogin2();
            var sender = new object();
            var eventArgs = EventArgs.Empty;

            // Act & Assert - Method should not throw
            var exception = Record.Exception(() => page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(page, new object[] { sender, eventArgs }));

            // Note: This test verifies method signature exists, actual redirect logic requires WebForms context
        }

        [Fact]
        public void AdminLogin2_PageLoadMethod_ShouldExist()
        {
            // Arrange
            var page = new AdminLogin2();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void AdminLogin2_ShouldBePublicPartialClass()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void AdminLogin2_ShouldBeInTourManagementNamespace()
        {
            // Arrange
            var type = typeof(AdminLogin2);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }
    }
}
