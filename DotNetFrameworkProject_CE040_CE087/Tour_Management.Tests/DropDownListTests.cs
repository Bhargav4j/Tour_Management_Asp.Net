using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class DropDownListTests
    {
        [Fact]
        public void DropDownList_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var dropDownList = new DropDownList();

            // Assert
            Assert.NotNull(dropDownList);
        }

        [Fact]
        public void DropDownList_SelectedValue_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var dropDownList = new DropDownList();

            // Act & Assert
            Assert.Equal("", dropDownList.SelectedValue);
        }

        [Fact]
        public void DropDownList_SelectedValue_ShouldBeSettable()
        {
            // Arrange
            var dropDownList = new DropDownList();
            string testValue = "value1";

            // Act
            dropDownList.SelectedValue = testValue;

            // Assert
            Assert.Equal(testValue, dropDownList.SelectedValue);
        }

        [Fact]
        public void DropDownList_Text_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var dropDownList = new DropDownList();

            // Act & Assert
            Assert.Equal("", dropDownList.Text);
        }

        [Fact]
        public void DropDownList_Text_ShouldBeSettable()
        {
            // Arrange
            var dropDownList = new DropDownList();
            string testValue = "Test Text";

            // Act
            dropDownList.Text = testValue;

            // Assert
            Assert.Equal(testValue, dropDownList.Text);
        }

        [Fact]
        public void DropDownList_Items_ShouldNotBeNull()
        {
            // Arrange
            var dropDownList = new DropDownList();

            // Act & Assert
            Assert.NotNull(dropDownList.Items);
        }

        [Fact]
        public void DropDownList_ShouldInheritFromControl()
        {
            // Arrange
            var dropDownList = new DropDownList();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Control>(dropDownList);
        }
    }
}
