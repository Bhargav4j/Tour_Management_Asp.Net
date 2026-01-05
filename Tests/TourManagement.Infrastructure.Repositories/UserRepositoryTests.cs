using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _loggerMock;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public UserRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<UserRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);

        // Act
        var repository = new UserRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnActiveUsers()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user1 = new User { UserId = 1, Email = "user1@test.com", FirstName = "First1", LastName = "Last1", PasswordHash = "hash1", IsActive = true };
        var user2 = new User { UserId = 2, Email = "user2@test.com", FirstName = "First2", LastName = "Last2", PasswordHash = "hash2", IsActive = true };
        var user3 = new User { UserId = 3, Email = "user3@test.com", FirstName = "First3", LastName = "Last3", PasswordHash = "hash3", IsActive = false };

        await context.Users.AddRangeAsync(user1, user2, user3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = false };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { UserId = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = false };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "new@test.com", FirstName = "New", LastName = "User", PasswordHash = "hash", IsActive = true };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.UserId > 0);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "original@test.com", FirstName = "Original", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        user.FirstName = "Updated";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.UserId);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkUserAsInactive()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.UserId);

        // Assert
        var deletedUser = await context.Users.FindAsync(user.UserId);
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.UserId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = false };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.UserId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = true };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.EmailExistsAsync("nonexistent@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "hash", IsActive = false };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.False(result);
    }
}
