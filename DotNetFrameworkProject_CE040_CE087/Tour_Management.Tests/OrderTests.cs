using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new Order();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void Order_ShouldInheritFromPage()
        {
            // Arrange
            var page = new Order();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new Order();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void BtnClick_ShouldExist()
        {
            // Arrange
            var page = new Order();
            var methodInfo = page.GetType().GetMethod("btn_click",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void Order_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void Order_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(Order);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void Order_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(Order).GetMethod("Page_Load",
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
        public void Order_BtnClickMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(Order).GetMethod("btn_click",
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
        public void Order_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(Order).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
