using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class ListItemTests
    {
        [Fact]
        public void ListItem_DefaultConstructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var listItem = new ListItem();

            // Assert
            Assert.NotNull(listItem);
        }

        [Fact]
        public void ListItem_ParameterizedConstructor_ShouldSetTextAndValue()
        {
            // Arrange
            string text = "Item Text";
            string value = "item_value";

            // Act
            var listItem = new ListItem(text, value);

            // Assert
            Assert.Equal(text, listItem.Text);
            Assert.Equal(value, listItem.Value);
        }

        [Fact]
        public void ListItem_Text_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var listItem = new ListItem();

            // Act & Assert
            Assert.Equal("", listItem.Text);
        }

        [Fact]
        public void ListItem_Value_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var listItem = new ListItem();

            // Act & Assert
            Assert.Equal("", listItem.Value);
        }

        [Fact]
        public void ListItem_Text_ShouldBeSettable()
        {
            // Arrange
            var listItem = new ListItem();
            string testText = "New Text";

            // Act
            listItem.Text = testText;

            // Assert
            Assert.Equal(testText, listItem.Text);
        }

        [Fact]
        public void ListItem_Value_ShouldBeSettable()
        {
            // Arrange
            var listItem = new ListItem();
            string testValue = "new_value";

            // Act
            listItem.Value = testValue;

            // Assert
            Assert.Equal(testValue, listItem.Value);
        }

        [Fact]
        public void ListItem_ParameterizedConstructor_WithEmptyStrings_ShouldSetEmptyStrings()
        {
            // Arrange & Act
            var listItem = new ListItem("", "");

            // Assert
            Assert.Equal("", listItem.Text);
            Assert.Equal("", listItem.Value);
        }
    }
}
