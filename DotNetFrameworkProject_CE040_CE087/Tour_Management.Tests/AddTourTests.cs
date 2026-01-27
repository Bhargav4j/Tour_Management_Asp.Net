using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class AddTourTests
    {
        [Fact]
        public void AddTour_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new AddTour();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void AddTour_ShouldInheritFromPage()
        {
            // Arrange
            var page = new AddTour();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new AddTour();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void RegisterClick_ShouldExist()
        {
            // Arrange
            var page = new AddTour();
            var methodInfo = page.GetType().GetMethod("Register_Click",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void AddTour_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void AddTour_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(AddTour);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void AddTour_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(AddTour).GetMethod("Page_Load",
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
        public void AddTour_RegisterClickMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(AddTour).GetMethod("Register_Click",
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
        public void AddTour_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(AddTour).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
