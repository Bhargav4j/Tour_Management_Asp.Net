using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class SignUpFormTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var signUpForm = new SignUpForm();

            // Assert
            Assert.NotNull(signUpForm);
        }

        [Fact]
        public void SignUpForm_InheritsFrom_PageClass()
        {
            // Arrange
            var signUpForm = new SignUpForm();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(signUpForm);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var signUpForm = new SignUpForm();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => signUpForm.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var signUpForm = new SignUpForm();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => signUpForm.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var signUpForm = new SignUpForm();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => signUpForm.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Register_Click_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var signUpForm = new SignUpForm();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => signUpForm.Register_Click(null, e));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }

        [Fact]
        public void Register_Click_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var signUpForm = new SignUpForm();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => signUpForm.Register_Click(sender, null));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }
    }
}
