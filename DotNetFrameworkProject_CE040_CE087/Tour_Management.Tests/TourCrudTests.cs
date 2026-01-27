using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class TourCrudTests
    {
        [Fact]
        public void TourCrud_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new TourCrud();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void TourCrud_ShouldInheritFromPage()
        {
            // Arrange
            var page = new TourCrud();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new TourCrud();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void RefreshData_ShouldExist()
        {
            // Arrange
            var page = new TourCrud();
            var methodInfo = page.GetType().GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void TourCrud_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void TourCrud_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(TourCrud);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void TourCrud_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(TourCrud).GetMethod("Page_Load",
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
        public void TourCrud_RefreshDataMethod_ShouldBePublic()
        {
            // Arrange
            var methodInfo = typeof(TourCrud).GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
            Assert.True(methodInfo.IsPublic);
        }

        [Fact]
        public void TourCrud_RefreshDataMethod_ShouldHaveNoParameters()
        {
            // Arrange
            var methodInfo = typeof(TourCrud).GetMethod("refreshdata",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var parameters = methodInfo?.GetParameters();

            // Assert
            Assert.NotNull(methodInfo);
            Assert.NotNull(parameters);
            Assert.Empty(parameters);
        }

        [Fact]
        public void TourCrud_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(TourCrud).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
