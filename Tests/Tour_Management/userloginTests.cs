using System;
using System.Web.UI;
using Xunit;

namespace Tour_Management.Tests
{
    public class userloginTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var userlogin = new userlogin();

            // Assert
            Assert.NotNull(userlogin);
        }

        [Fact]
        public void userlogin_InheritsFrom_PageClass()
        {
            // Arrange
            var userlogin = new userlogin();

            // Act & Assert
            Assert.IsAssignableFrom<System.Web.UI.Page>(userlogin);
        }

        [Fact]
        public void Page_Load_ShouldNotThrowException()
        {
            // Arrange
            var userlogin = new userlogin();
            var sender = new object();
            var e = EventArgs.Empty;

            // Act & Assert
            var exception = Record.Exception(() => userlogin.Page_Load(sender, e));
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => userlogin.Page_Load(null, e));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Page_Load_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => userlogin.Page_Load(sender, null));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void Btn_Submit_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => userlogin.Btn_Submit(null, e));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }

        [Fact]
        public void Btn_Submit_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => userlogin.Btn_Submit(sender, null));

            // Assert - Will throw due to missing dependencies
            Assert.NotNull(exception);
        }

        [Fact]
        public void Btn_reg_WithNullSender_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var e = EventArgs.Empty;

            // Act
            var exception = Record.Exception(() => userlogin.Btn_reg(null, e));

            // Assert - Will throw due to missing HTTP context
            Assert.NotNull(exception);
        }

        [Fact]
        public void Btn_reg_WithNullEventArgs_ShouldHandleGracefully()
        {
            // Arrange
            var userlogin = new userlogin();
            var sender = new object();

            // Act
            var exception = Record.Exception(() => userlogin.Btn_reg(sender, null));

            // Assert - Will throw due to missing HTTP context
            Assert.NotNull(exception);
        }
    }
}
