using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Exceptions.Tests;

public class ValidationExceptionTests
{
    [Fact]
    public void ValidationException_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Validation failed";

        // Act
        var exception = new ValidationException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.NotNull(exception.Errors);
        Assert.Empty(exception.Errors);
    }

    [Fact]
    public void ValidationException_WithErrors_ShouldSetDefaultMessageAndErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required", "Email is invalid" } },
            { "Password", new[] { "Password is too short" } }
        };
        var expectedMessage = "One or more validation errors occurred.";

        // Act
        var exception = new ValidationException(errors);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.NotNull(exception.Errors);
        Assert.Equal(2, exception.Errors.Count);
        Assert.Equal(errors["Email"], exception.Errors["Email"]);
        Assert.Equal(errors["Password"], exception.Errors["Password"]);
    }

    [Fact]
    public void ValidationException_WithEmptyErrors_ShouldInitializeEmptyErrorsDictionary()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>();

        // Act
        var exception = new ValidationException(errors);

        // Assert
        Assert.NotNull(exception.Errors);
        Assert.Empty(exception.Errors);
    }

    [Fact]
    public void ValidationException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new ValidationException("Test message");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void ValidationException_Errors_ShouldBeReadOnly()
    {
        // Arrange
        var message = "Validation error";
        var exception = new ValidationException(message);

        // Act & Assert
        Assert.NotNull(exception.Errors);
        Assert.IsAssignableFrom<IDictionary<string, string[]>>(exception.Errors);
    }

    [Fact]
    public void ValidationException_WithMultipleErrorsPerField_ShouldStoreAllErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Error1", "Error2", "Error3" } }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        Assert.Equal(3, exception.Errors["Name"].Length);
        Assert.Contains("Error1", exception.Errors["Name"]);
        Assert.Contains("Error2", exception.Errors["Name"]);
        Assert.Contains("Error3", exception.Errors["Name"]);
    }
}
