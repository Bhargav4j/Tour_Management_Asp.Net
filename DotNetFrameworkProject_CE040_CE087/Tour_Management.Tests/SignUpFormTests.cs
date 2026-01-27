using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class SignUpFormTests
    {
        [Fact]
        public void SignUpForm_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new SignUpForm();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void SignUpForm_ShouldInheritFromPage()
        {
            // Arrange
            var page = new SignUpForm();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new SignUpForm();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void RegisterClick_ShouldExist()
        {
            // Arrange
            var page = new SignUpForm();
            var methodInfo = page.GetType().GetMethod("Register_Click",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void SignUpForm_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void SignUpForm_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(SignUpForm);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void SignUpForm_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(SignUpForm).GetMethod("Page_Load",
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
        public void SignUpForm_RegisterClickMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(SignUpForm).GetMethod("Register_Click",
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
        public void SignUpForm_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(SignUpForm).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
