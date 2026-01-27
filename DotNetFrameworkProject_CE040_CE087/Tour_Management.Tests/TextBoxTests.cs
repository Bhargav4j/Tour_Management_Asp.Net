using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class TextBoxTests
    {
        [Fact]
        public void TextBox_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var textBox = new TextBox();

            // Assert
            Assert.NotNull(textBox);
        }

        [Fact]
        public void TextBox_Text_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var textBox = new TextBox();

            // Act & Assert
            Assert.Equal("", textBox.Text);
        }

        [Fact]
        public void TextBox_Text_ShouldBeSettable()
        {
            // Arrange
            var textBox = new TextBox();
            string testValue = "Test Value";

            // Act
            textBox.Text = testValue;

            // Assert
            Assert.Equal(testValue, textBox.Text);
        }

        [Fact]
        public void TextBox_Text_SetEmptyString_ShouldStoreEmptyString()
        {
            // Arrange
            var textBox = new TextBox();

            // Act
            textBox.Text = "";

            // Assert
            Assert.Equal("", textBox.Text);
        }

        [Fact]
        public void TextBox_ShouldInheritFromControl()
        {
            // Arrange
            var textBox = new TextBox();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Control>(textBox);
        }
    }
}
