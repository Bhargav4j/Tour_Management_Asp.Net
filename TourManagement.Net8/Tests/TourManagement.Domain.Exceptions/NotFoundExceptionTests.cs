using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Exceptions.Tests;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_CreatesException()
    {
        // Arrange
        var message = "Entity not found";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithEntityAndKey_CreatesException()
    {
        // Arrange
        var entity = "User";
        var key = 123;

        // Act
        var exception = new NotFoundException(entity, key);

        // Assert
        Assert.NotNull(exception);
        Assert.Contains(entity, exception.Message);
        Assert.Contains(key.ToString(), exception.Message);
    }

    [Fact]
    public void NotFoundException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException("Test");

        // Assert
        Assert.IsAssignableFrom<System.Exception>(exception);
    }
}
