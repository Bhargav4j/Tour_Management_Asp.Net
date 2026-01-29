using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Tests.Domain.Exceptions;

public class TourManagementExceptionTests
{
    [Fact]
    public void TourManagementException_DefaultConstructor_CreatesInstance()
    {
        var exception = new TourManagementException();

        Assert.NotNull(exception);
        Assert.IsType<TourManagementException>(exception);
    }

    [Fact]
    public void TourManagementException_WithMessage_StoresMessage()
    {
        var message = "Test error message";

        var exception = new TourManagementException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void TourManagementException_WithMessageAndInnerException_StoresBoth()
    {
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");

        var exception = new TourManagementException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void EntityNotFoundException_WithEntityNameAndKey_CreatesCorrectMessage()
    {
        var entityName = "Tour";
        var key = 123;

        var exception = new EntityNotFoundException(entityName, key);

        Assert.Contains("Tour", exception.Message);
        Assert.Contains("123", exception.Message);
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_InheritsFromTourManagementException()
    {
        var exception = new EntityNotFoundException("User", "test@example.com");

        Assert.IsAssignableFrom<TourManagementException>(exception);
    }

    [Fact]
    public void EntityNotFoundException_WithStringKey_FormatsCorrectly()
    {
        var entityName = "User";
        var key = "test@example.com";

        var exception = new EntityNotFoundException(entityName, key);

        Assert.Contains("User", exception.Message);
        Assert.Contains("test@example.com", exception.Message);
    }

    [Fact]
    public void EntityAlreadyExistsException_WithEntityNameAndKey_CreatesCorrectMessage()
    {
        var entityName = "User";
        var key = "duplicate@example.com";

        var exception = new EntityAlreadyExistsException(entityName, key);

        Assert.Contains("User", exception.Message);
        Assert.Contains("duplicate@example.com", exception.Message);
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void EntityAlreadyExistsException_InheritsFromTourManagementException()
    {
        var exception = new EntityAlreadyExistsException("Tour", 456);

        Assert.IsAssignableFrom<TourManagementException>(exception);
    }

    [Fact]
    public void EntityAlreadyExistsException_WithIntKey_FormatsCorrectly()
    {
        var entityName = "Tour";
        var key = 789;

        var exception = new EntityAlreadyExistsException(entityName, key);

        Assert.Contains("Tour", exception.Message);
        Assert.Contains("789", exception.Message);
    }

    [Fact]
    public void ValidationException_WithMessage_StoresMessage()
    {
        var message = "Validation failed for field X";

        var exception = new ValidationException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void ValidationException_InheritsFromTourManagementException()
    {
        var exception = new ValidationException("Test validation error");

        Assert.IsAssignableFrom<TourManagementException>(exception);
    }

    [Fact]
    public void AllCustomExceptions_InheritFromException()
    {
        var tourException = new TourManagementException();
        var notFoundException = new EntityNotFoundException("Test", 1);
        var existsException = new EntityAlreadyExistsException("Test", 2);
        var validationException = new ValidationException("Test");

        Assert.IsAssignableFrom<Exception>(tourException);
        Assert.IsAssignableFrom<Exception>(notFoundException);
        Assert.IsAssignableFrom<Exception>(existsException);
        Assert.IsAssignableFrom<Exception>(validationException);
    }
}
