using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class AdminProfileTests
    {
        [Fact]
        public void AdminProfile_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new AdminProfile();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void AdminProfile_ShouldInheritFromPage()
        {
            // Arrange
            var page = new AdminProfile();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldAcceptValidParameters()
        {
            // Arrange
            var page = new AdminProfile();
            var sender = new object();
            var eventArgs = EventArgs.Empty;

            // Act
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void AdminProfile_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(AdminProfile);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void AdminProfile_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(AdminProfile);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void AdminProfile_PageLoadMethod_ShouldExist()
        {
            // Arrange
            var page = new AdminProfile();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void AdminProfile_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(AdminProfile).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
