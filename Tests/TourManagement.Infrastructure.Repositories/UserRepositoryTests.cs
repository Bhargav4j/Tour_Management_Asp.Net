using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;

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

    [Fact]
    public void UserRepository_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new UserRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void UserRepository_Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void UserRepository_Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user1 = new User { Email = "user1@test.com", FirstName = "User1", LastName = "Test1", IsActive = true, CreatedDate = DateTime.UtcNow };
        var user2 = new User { Email = "user2@test.com", FirstName = "User2", LastName = "Test2", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) };
        var user3 = new User { Email = "user3@test.com", FirstName = "User3", LastName = "Test3", IsActive = false, CreatedDate = DateTime.UtcNow };

        context.Users.AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Email == "user1@test.com");
        Assert.Contains(result, u => u.Email == "user2@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "findme@test.com", FirstName = "Find", LastName = "Me", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("findme@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("findme@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User
        {
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser@test.com", result.Email);

        var savedUser = await context.Users.FindAsync(result.Id);
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "update@test.com", FirstName = "Original", LastName = "Name", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Entry(user).State = EntityState.Detached;

        // Act
        user.FirstName = "Updated";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "delete@test.com", FirstName = "Delete", LastName = "Me", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.Id);

        // Assert
        var deletedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
        Assert.NotNull(deletedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "exists@test.com", FirstName = "Exists", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ReturnsUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user1 = new User { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true, CreatedDate = DateTime.UtcNow };
        var user2 = new User { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, CreatedDate = DateTime.UtcNow };
        var user3 = new User { Email = "bob@test.com", FirstName = "Bob", LastName = "Johnson", IsActive = true, CreatedDate = DateTime.UtcNow };

        context.Users.AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.FirstName == "John");
        Assert.Contains(result, u => u.LastName == "Johnson");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NoMatch");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
