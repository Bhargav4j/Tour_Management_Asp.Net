using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private TourManagementDbContext CreateContext()
    {
        return new TourManagementDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true },
            new User { Id = 2, Username = "user2", Email = "user2@test.com", IsActive = true },
            new User { Id = 3, Username = "user3", Email = "user3@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.DoesNotContain(result, u => u.Id == 3);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithValidUsername_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Username = "john_doe", Email = "john@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUsernameAsync("john_doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john_doe", result.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Username = "inactive", Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByUsernameAsync("inactive");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Username = "user1", Email = "user1@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("user1@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Username = "inactive", Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Username = "newuser", Email = "newuser@test.com", PasswordHash = "hash123" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser", result.Username);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserInDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User { Username = "original", Email = "original@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        user.Username = "updated";

        // Act
        var result = await repository.UpdateAsync(user);

        // Assert
        Assert.Equal("updated", result.Username);
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.Equal("updated", updatedUser?.Username);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateContext();
        var user = new User { Username = "user1", Email = "user1@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(user.Id);

        // Assert
        Assert.True(result);
        var deletedUser = await context.Users.FindAsync(user.Id);
        Assert.False(deletedUser?.IsActive);
        Assert.NotNull(deletedUser?.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Id = 1, Username = "user1", Email = "user1@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingUsername_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "john_doe", Email = "john@test.com", IsActive = true },
            new User { Username = "jane_doe", Email = "jane@test.com", IsActive = true },
            new User { Username = "bob_smith", Email = "bob@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("doe");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Contains("doe", u.Username));
    }

    [Fact]
    public async Task SearchAsync_WithMatchingEmail_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "user1", Email = "john@example.com", IsActive = true },
            new User { Username = "user2", Email = "jane@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("example");

        // Assert
        Assert.Single(result);
        Assert.Contains("example", result.First().Email);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingFullName_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "user1", Email = "user1@test.com", FullName = "John Smith", IsActive = true },
            new User { Username = "user2", Email = "user2@test.com", FullName = "Jane Doe", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("John Smith", result.First().FullName);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllActiveUsers()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.AddRange(
            new User { Username = "user1", Email = "user1@test.com", IsActive = true },
            new User { Username = "user2", Email = "user2@test.com", IsActive = true },
            new User { Username = "user3", Email = "user3@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        context.Users.Add(new User { Username = "user1", Email = "user1@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("nonexistent");

        // Assert
        Assert.Empty(result);
    }
}
