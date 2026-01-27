using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class allbookingTests
    {
        [Fact]
        public void allbooking_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new allbooking();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void allbooking_ShouldInheritFromPage()
        {
            // Arrange
            var page = new allbooking();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new allbooking();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void allbooking_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void allbooking_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(allbooking);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void allbooking_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(allbooking).GetMethod("Page_Load",
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
        public void allbooking_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(allbooking).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
