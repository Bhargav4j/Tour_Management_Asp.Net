using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class LabelTests
    {
        [Fact]
        public void Label_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var label = new Label();

            // Assert
            Assert.NotNull(label);
        }

        [Fact]
        public void Label_Text_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var label = new Label();

            // Act & Assert
            Assert.Equal("", label.Text);
        }

        [Fact]
        public void Label_Text_ShouldBeSettable()
        {
            // Arrange
            var label = new Label();
            string testValue = "Test Label";

            // Act
            label.Text = testValue;

            // Assert
            Assert.Equal(testValue, label.Text);
        }

        [Fact]
        public void Label_Text_SetEmptyString_ShouldStoreEmptyString()
        {
            // Arrange
            var label = new Label();

            // Act
            label.Text = "";

            // Assert
            Assert.Equal("", label.Text);
        }

        [Fact]
        public void Label_ShouldInheritFromControl()
        {
            // Arrange
            var label = new Label();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Control>(label);
        }
    }
}
