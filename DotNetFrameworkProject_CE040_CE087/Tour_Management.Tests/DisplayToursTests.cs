using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class DisplayToursTests
    {
        [Fact]
        public void DisplayTours_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new DisplayTours();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void DisplayTours_ShouldInheritFromPage()
        {
            // Arrange
            var page = new DisplayTours();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new DisplayTours();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void DisplayTours_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void DisplayTours_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(DisplayTours);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void DisplayTours_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(DisplayTours).GetMethod("Page_Load",
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
        public void DisplayTours_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(DisplayTours).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
