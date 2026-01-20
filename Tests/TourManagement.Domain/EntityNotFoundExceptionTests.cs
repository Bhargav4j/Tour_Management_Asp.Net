using Xunit;
using TourManagement.Domain.Exceptions;

namespace TourManagement.Domain.Tests;

public class EntityNotFoundExceptionTests
{
    [Fact]
    public void EntityNotFoundException_Constructor_SetsEntityNameAndId()
    {
        // Arrange
        var entityName = "Tour";
        var entityId = 123;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal(entityName, exception.EntityName);
        Assert.Equal(entityId, exception.EntityId);
    }

    [Fact]
    public void EntityNotFoundException_Constructor_SetsCorrectMessage()
    {
        // Arrange
        var entityName = "Booking";
        var entityId = 456;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal("Booking with ID 456 was not found.", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_WithDifferentEntity_SetsCorrectProperties()
    {
        // Arrange
        var entityName = "UserInfo";
        var entityId = 789;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal("UserInfo", exception.EntityName);
        Assert.Equal(789, exception.EntityId);
        Assert.Equal("UserInfo with ID 789 was not found.", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_WithZeroId_HandlesCorrectly()
    {
        // Arrange
        var entityName = "Tour";
        var entityId = 0;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal("Tour", exception.EntityName);
        Assert.Equal(0, exception.EntityId);
        Assert.Equal("Tour with ID 0 was not found.", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_WithNegativeId_HandlesCorrectly()
    {
        // Arrange
        var entityName = "Booking";
        var entityId = -1;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal("Booking", exception.EntityName);
        Assert.Equal(-1, exception.EntityId);
        Assert.Equal("Booking with ID -1 was not found.", exception.Message);
    }

    [Fact]
    public void EntityNotFoundException_InheritsFromException()
    {
        // Arrange
        var entityName = "Tour";
        var entityId = 100;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void EntityNotFoundException_CanBeThrown()
    {
        // Arrange
        var entityName = "UserInfo";
        var entityId = 200;

        // Act
        void ThrowException() => throw new EntityNotFoundException(entityName, entityId);

        // Assert
        var exception = Assert.Throws<EntityNotFoundException>(ThrowException);

        Assert.Equal("UserInfo", exception.EntityName);
        Assert.Equal(200, exception.EntityId);
    }

    [Fact]
    public void EntityNotFoundException_WithEmptyEntityName_SetsCorrectProperties()
    {
        // Arrange
        var entityName = "";
        var entityId = 300;

        // Act
        var exception = new EntityNotFoundException(entityName, entityId);

        // Assert
        Assert.Equal("", exception.EntityName);
        Assert.Equal(300, exception.EntityId);
        Assert.Equal(" with ID 300 was not found.", exception.Message);
    }
}
