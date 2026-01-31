using System;
using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Exceptions.Tests
{
    /// <summary>
    /// Tests for BusinessException
    /// </summary>
    public class BusinessExceptionTests
    {
        [Fact]
        public void BusinessException_WithMessage_ShouldCreateExceptionWithMessage()
        {
            // Arrange
            var message = "Business rule violation occurred";

            // Act
            var exception = new BusinessException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void BusinessException_WithMessageAndInnerException_ShouldCreateExceptionWithBoth()
        {
            // Arrange
            var message = "Business rule violation";
            var innerException = new InvalidOperationException("Inner error");

            // Act
            var exception = new BusinessException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void BusinessException_ShouldBeInstanceOfException()
        {
            // Arrange & Act
            var exception = new BusinessException("Test message");

            // Assert
            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Theory]
        [InlineData("Invalid booking date")]
        [InlineData("Tour capacity exceeded")]
        [InlineData("Payment processing failed")]
        public void BusinessException_WithDifferentMessages_ShouldCreateExceptionCorrectly(string message)
        {
            // Arrange & Act
            var exception = new BusinessException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void BusinessException_CanBeThrown()
        {
            // Arrange
            var message = "Test business exception";

            // Act
            var exception = new BusinessException(message);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void BusinessException_WithInnerException_CanBeThrown()
        {
            // Arrange
            var message = "Business error";
            var innerException = new ArgumentException("Argument error");

            // Act
            var exception = new BusinessException(message, innerException);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void BusinessException_CanBeCaught()
        {
            // Arrange
            var message = "Caught business exception";
            var caught = false;

            // Act
            try
            {
                throw new BusinessException(message);
            }
            catch (BusinessException ex)
            {
                caught = true;
                Assert.Equal(message, ex.Message);
            }

            // Assert
            Assert.True(caught);
        }

        [Fact]
        public void BusinessException_WithEmptyMessage_ShouldCreateException()
        {
            // Arrange
            var message = string.Empty;

            // Act
            var exception = new BusinessException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void BusinessException_InnerExceptionMessage_ShouldBeAccessible()
        {
            // Arrange
            var message = "Outer message";
            var innerMessage = "Inner exception message";
            var innerException = new Exception(innerMessage);

            // Act
            var exception = new BusinessException(message, innerException);

            // Assert
            Assert.Equal(innerMessage, exception.InnerException.Message);
        }

        [Fact]
        public void BusinessException_WithNullInnerException_ShouldNotThrow()
        {
            // Arrange
            var message = "Test message";
            Exception? innerException = null;

            // Act & Assert
            var exception = new BusinessException(message, innerException!);
            Assert.NotNull(exception);
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void BusinessException_AsBaseException_ShouldBeCatchable()
        {
            // Arrange
            var message = "Business exception as base";
            var caught = false;

            // Act
            try
            {
                throw new BusinessException(message);
            }
            catch (Exception ex)
            {
                caught = true;
                Assert.IsType<BusinessException>(ex);
                Assert.Equal(message, ex.Message);
            }

            // Assert
            Assert.True(caught);
        }
    }
}
