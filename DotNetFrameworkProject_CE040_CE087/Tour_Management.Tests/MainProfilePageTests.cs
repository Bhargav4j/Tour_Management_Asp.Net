using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class MainProfilePageTests
    {
        [Fact]
        public void MainProfilePage_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new MainProfilePage();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void MainProfilePage_ShouldInheritFromPage()
        {
            // Arrange
            var page = new MainProfilePage();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldHaveCorrectSignature()
        {
            // Arrange
            var page = new MainProfilePage();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void MainProfilePage_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(MainProfilePage);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void MainProfilePage_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(MainProfilePage);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void MainProfilePage_PageLoadMethod_ShouldBeProtected()
        {
            // Arrange
            var methodInfo = typeof(MainProfilePage).GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
            Assert.False(methodInfo.IsPublic);
        }

        [Fact]
        public void MainProfilePage_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(MainProfilePage).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
