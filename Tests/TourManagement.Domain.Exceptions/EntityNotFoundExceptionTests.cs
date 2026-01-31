using System;
using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Exceptions.Tests
{
    /// <summary>
    /// Tests for EntityNotFoundException
    /// </summary>
    public class EntityNotFoundExceptionTests
    {
        [Fact]
        public void EntityNotFoundException_WithEntityNameAndKey_ShouldCreateExceptionWithFormattedMessage()
        {
            // Arrange
            var entityName = "User";
            var key = 123;

            // Act
            var exception = new EntityNotFoundException(entityName, key);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_WithCustomMessage_ShouldCreateExceptionWithMessage()
        {
            // Arrange
            var customMessage = "Custom entity not found message";

            // Act
            var exception = new EntityNotFoundException(customMessage);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(customMessage, exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_ShouldBeInstanceOfException()
        {
            // Arrange & Act
            var exception = new EntityNotFoundException("Test", 1);

            // Assert
            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Theory]
        [InlineData("Tour", 1)]
        [InlineData("Booking", 999)]
        [InlineData("Admin", 0)]
        public void EntityNotFoundException_WithDifferentEntityNamesAndKeys_ShouldFormatMessageCorrectly(string entityName, int key)
        {
            // Arrange & Act
            var exception = new EntityNotFoundException(entityName, key);

            // Assert
            Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_WithStringKey_ShouldFormatMessageCorrectly()
        {
            // Arrange
            var entityName = "User";
            var key = "user@example.com";

            // Act
            var exception = new EntityNotFoundException(entityName, key);

            // Assert
            Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_WithGuidKey_ShouldFormatMessageCorrectly()
        {
            // Arrange
            var entityName = "Order";
            var key = Guid.NewGuid();

            // Act
            var exception = new EntityNotFoundException(entityName, key);

            // Assert
            Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_CanBeThrown()
        {
            // Arrange
            var entityName = "TestEntity";
            var key = 42;

            // Act
            var exception = new EntityNotFoundException(entityName, key);

            // Assert
            Assert.NotNull(exception);
            Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_CanBeCaught()
        {
            // Arrange
            var entityName = "TestEntity";
            var key = 100;
            var caught = false;

            // Act
            try
            {
                throw new EntityNotFoundException(entityName, key);
            }
            catch (EntityNotFoundException ex)
            {
                caught = true;
                Assert.Equal($"Entity '{entityName}' with key '{key}' was not found.", ex.Message);
            }

            // Assert
            Assert.True(caught);
        }

        [Fact]
        public void EntityNotFoundException_WithEmptyMessage_ShouldCreateException()
        {
            // Arrange
            var message = string.Empty;

            // Act
            var exception = new EntityNotFoundException(message);

            // Assert
            Assert.Equal(message, exception.Message);
        }

        [Fact]
        public void EntityNotFoundException_WithNullKey_ShouldFormatMessageCorrectly()
        {
            // Arrange
            var entityName = "TestEntity";
            object? key = null;

            // Act
            var exception = new EntityNotFoundException(entityName, key!);

            // Assert
            Assert.Contains(entityName, exception.Message);
        }
    }
}
