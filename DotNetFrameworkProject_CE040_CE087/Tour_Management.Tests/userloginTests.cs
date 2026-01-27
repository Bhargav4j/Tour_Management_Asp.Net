using System;
using Xunit;
using Tour_Management;

namespace Tour_Management.Tests
{
    public class userloginTests
    {
        [Fact]
        public void userlogin_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var page = new userlogin();

            // Assert
            Assert.NotNull(page);
        }

        [Fact]
        public void userlogin_ShouldInheritFromPage()
        {
            // Arrange
            var page = new userlogin();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(page);
        }

        [Fact]
        public void PageLoad_ShouldExist()
        {
            // Arrange
            var page = new userlogin();
            var methodInfo = page.GetType().GetMethod("Page_Load",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void BtnSubmit_ShouldExist()
        {
            // Arrange
            var page = new userlogin();
            var methodInfo = page.GetType().GetMethod("Btn_Submit",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void BtnReg_ShouldExist()
        {
            // Arrange
            var page = new userlogin();
            var methodInfo = page.GetType().GetMethod("Btn_reg",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            Assert.NotNull(methodInfo);
        }

        [Fact]
        public void userlogin_ShouldBePublicClass()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.True(type.IsPublic);
            Assert.True(type.IsClass);
        }

        [Fact]
        public void userlogin_ShouldBeInCorrectNamespace()
        {
            // Arrange
            var type = typeof(userlogin);

            // Assert
            Assert.Equal("Tour_Management", type.Namespace);
        }

        [Fact]
        public void userlogin_PageLoadMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(userlogin).GetMethod("Page_Load",
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
        public void userlogin_BtnSubmitMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(userlogin).GetMethod("Btn_Submit",
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
        public void userlogin_BtnRegMethod_ShouldAcceptCorrectParameters()
        {
            // Arrange
            var methodInfo = typeof(userlogin).GetMethod("Btn_reg",
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
        public void userlogin_ShouldHaveParameterlessConstructor()
        {
            // Arrange & Act
            var constructor = typeof(userlogin).GetConstructor(Type.EmptyTypes);

            // Assert
            Assert.NotNull(constructor);
        }
    }
}
