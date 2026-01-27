using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class mybookingTests
    {
        [Fact]
        public void mybooking_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new mybooking();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void mybooking_ShouldInheritFromPage()
        {
            // Arrange
            var page = new mybooking();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new mybooking();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void mybooking_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void mybooking_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(mybooking);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void mybooking_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(mybooking).GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var parameters = methodInfo?.GetParameters();

            // Assert
            Assert.NotNull(methodInfo);
            Assert.NotNull(parameters);
            Assert.Equal(2, parameters.Length);
            Assert.Equal(typeof(object), parameters[0].ParameterType);
            Assert.Equal(typeof(EventArgs), parameters[1].ParameterType);
        }

        [Fact]
        public void mybooking_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(mybooking).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
