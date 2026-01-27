using System;
using System.Web.UI.WebControls;
using Xunit;

namespace SystemWebUI.Tests
{
    public class FileUploadTests
    {
        [Fact]
        public void FileUpload_Constructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var fileUpload = new FileUpload();

            // Assert
            Assert.NotNull(fileUpload);
        }

        [Fact]
        public void FileUpload_HasFile_DefaultValue_ShouldBeFalse()
        {
            // Arrange
            var fileUpload = new FileUpload();

            // Act & Assert
            Assert.False(fileUpload.HasFile);
        }

        [Fact]
        public void FileUpload_HasFile_ShouldBeSettable()
        {
            // Arrange
            var fileUpload = new FileUpload();

            // Act
            fileUpload.HasFile = true;

            // Assert
            Assert.True(fileUpload.HasFile);
        }

        [Fact]
        public void FileUpload_FileName_DefaultValue_ShouldBeEmptyString()
        {
            // Arrange
            var fileUpload = new FileUpload();

            // Act & Assert
            Assert.Equal("", fileUpload.FileName);
        }

        [Fact]
        public void FileUpload_FileName_ShouldBeSettable()
        {
            // Arrange
            var fileUpload = new FileUpload();
            string fileName = "test.jpg";

            // Act
            fileUpload.FileName = fileName;

            // Assert
            Assert.Equal(fileName, fileUpload.FileName);
        }

        [Fact]
        public void FileUpload_SaveAs_ShouldNotThrow()
        {
            // Arrange
            var fileUpload = new FileUpload();
            string path = "/test/path/file.jpg";

            // Act & Assert
            var exception = Record.Exception(() => fileUpload.SaveAs(path));
            Assert.Null(exception);
        }

        [Fact]
        public void FileUpload_SaveAs_WithEmptyPath_ShouldNotThrow()
        {
            // Arrange
            var fileUpload = new FileUpload();

            // Act & Assert
            var exception = Record.Exception(() => fileUpload.SaveAs(""));
            Assert.Null(exception);
        }

        [Fact]
        public void FileUpload_ShouldInheritFromControl()
        {
            // Arrange
            var fileUpload = new FileUpload();

            // Assert
            Assert.IsAssignableFrom<System.Web.UI.Control>(fileUpload);
        }
    }
}
