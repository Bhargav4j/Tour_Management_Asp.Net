using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

/// <summary>
/// Unit tests for UserRepository
/// </summary>
public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Id = 1, Email = "user1@test.com", IsActive = true },
            new User { Id = 2, Email = "user2@test.com", IsActive = true },
            new User { Id = 3, Email = "user3@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User
        {
            Email = "new@test.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Old", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.FirstName = "New";
        await repository.UpdateAsync(user);

        // Assert
        var updated = await context.Users.FindAsync(user.Id);
        Assert.Equal("New", updated!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.Id);

        // Assert
        var deletedUser = await context.Users.FindAsync(user.Id);
        Assert.False(deletedUser!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistentEmail_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.EmailExistsAsync("nonexistent@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Email = "john@test.com", FirstName = "John", IsActive = true },
            new User { Email = "jane@test.com", FirstName = "Jane", IsActive = true },
            new User { Email = "bob@test.com", FirstName = "Bob", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("john@test.com", result.First().Email);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }
}
