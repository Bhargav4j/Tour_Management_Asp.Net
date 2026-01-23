using Xunit;
using TourManagement.Domain.Exceptions;
using System.Collections.Generic;

namespace TourManagement.Domain.Exceptions.Tests;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_CreatesException()
    {
        // Arrange
        var message = "Validation failed";

        // Act
        var exception = new ValidationException(message);

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithErrors_CreatesException()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Email", new[] { "Email is required" } },
            { "Password", new[] { "Password is too short" } }
        };

        // Act
        var exception = new ValidationException(errors);

        // Assert
        Assert.NotNull(exception);
        Assert.NotNull(exception.Errors);
        Assert.Equal(2, exception.Errors.Count);
        Assert.Contains("Email", exception.Errors.Keys);
        Assert.Contains("Password", exception.Errors.Keys);
    }

    [Fact]
    public void Errors_PropertyInitialized()
    {
        // Arrange & Act
        var exception = new ValidationException("Test");

        // Assert
        Assert.NotNull(exception.Errors);
        Assert.Empty(exception.Errors);
    }

    [Fact]
    public void ValidationException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new ValidationException("Test");

        // Assert
        Assert.IsAssignableFrom<System.Exception>(exception);
    }
}
