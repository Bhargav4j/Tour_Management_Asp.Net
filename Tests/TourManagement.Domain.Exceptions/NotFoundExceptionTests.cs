using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Exceptions.Tests;

public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Entity not found";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public void NotFoundException_WithNameAndKey_ShouldGenerateFormattedMessage()
    {
        // Arrange
        var name = "Tour";
        var key = 123;
        var expectedMessage = $"Entity \"{name}\" ({key}) was not found.";

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void NotFoundException_WithStringKey_ShouldGenerateFormattedMessage()
    {
        // Arrange
        var name = "User";
        var key = "test@example.com";
        var expectedMessage = $"Entity \"{name}\" ({key}) was not found.";

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException("Test message");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void NotFoundException_WithEmptyMessage_ShouldAcceptEmptyString()
    {
        // Arrange
        var message = string.Empty;

        // Act
        var exception = new NotFoundException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void NotFoundException_WithNullKey_ShouldGenerateFormattedMessage()
    {
        // Arrange
        var name = "Entity";
        object? key = null;
        var expectedMessage = $"Entity \"{name}\" ({key}) was not found.";

        // Act
        var exception = new NotFoundException(name, key!);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }
}
