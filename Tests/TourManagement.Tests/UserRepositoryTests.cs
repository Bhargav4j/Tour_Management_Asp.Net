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

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new UserRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.AddRange(
            new User { Email = "user1@test.com", FirstName = "Alice", IsActive = true },
            new User { Email = "user2@test.com", FirstName = "Bob", IsActive = true },
            new User { Email = "user3@test.com", FirstName = "Charlie", IsActive = false }
        );
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByFirstName()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.AddRange(
            new User { Email = "user1@test.com", FirstName = "Charlie", IsActive = true },
            new User { Email = "user2@test.com", FirstName = "Alice", IsActive = true },
            new User { Email = "user3@test.com", FirstName = "Bob", IsActive = true }
        );
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal("Alice", result[0].FirstName);
        Assert.Equal("Bob", result[1].FirstName);
        Assert.Equal("Charlie", result[2].FirstName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new User { Email = "test@example.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.Add(new User { Email = "test@example.com", FirstName = "John", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldAddAndReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "new@example.com", FirstName = "Jane" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
        Assert.Equal("Jane", result.FirstName);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "new@example.com", FirstName = "Jane" };

        // Act
        await repository.AddAsync(user);

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "new@example.com");
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new User { Email = "test@example.com", FirstName = "Original", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.Entry(user).State = EntityState.Detached;

        var repository = new UserRepository(context, _mockLogger.Object);
        user.FirstName = "Updated";

        // Act
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync("test@example.com");
        Assert.Equal("Updated", updatedUser!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldSetIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new User { Email = "test@example.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync("test@example.com");

        // Assert
        var deletedUser = await context.Users.FindAsync("test@example.com");
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new User { Email = "test@example.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync("test@example.com");

        // Assert
        var deletedUser = await context.Users.FindAsync("test@example.com");
        Assert.NotNull(deletedUser!.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldNotThrowException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync("nonexistent@example.com");
        // No exception should be thrown
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.Add(new User { Email = "test@example.com", FirstName = "John", IsActive = true });
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync("test@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync("nonexistent@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.Add(new User { Email = "test@example.com", FirstName = "John", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync("test@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            PasswordHash = "hashedPassword",
            IsActive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AuthenticateAsync("test@example.com", "password");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AuthenticateAsync("nonexistent@example.com", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Users.Add(new User
        {
            Email = "test@example.com",
            FirstName = "John",
            PasswordHash = "hashedPassword",
            IsActive = false
        });
        await context.SaveChangesAsync();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AuthenticateAsync("test@example.com", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithEmptyEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.AuthenticateAsync("", "password");

        // Assert
        Assert.Null(result);
    }
}
