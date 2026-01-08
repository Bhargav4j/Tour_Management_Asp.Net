using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.AddRange(
            new User { Email = "user1@test.com", FirstName = "User1", IsActive = true },
            new User { Email = "user2@test.com", FirstName = "User2", IsActive = true },
            new User { Email = "user3@test.com", FirstName = "User3", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActiveUsers_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.Add(new User { Email = "user@test.com", FirstName = "User", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = true };
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
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal("John", result.FirstName);
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
    public async Task GetByEmailAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "new@test.com", FirstName = "John", IsActive = true };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task AddAsync_SavesUserToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "new@test.com", FirstName = "John", IsActive = true };

        // Act
        await repository.AddAsync(user);

        // Assert
        var savedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("new@test.com", savedUser.Email);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "old@test.com", FirstName = "OldName", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.Entry(user).State = EntityState.Detached;

        // Act
        user.FirstName = "NewName";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("NewName", updatedUser.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "delete@test.com", FirstName = "John", IsActive = true };
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
    public async Task DeleteAsync_WithInvalidId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "test@test.com", FirstName = "John", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
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
    public async Task SearchAsync_ByEmail_ReturnsUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.AddRange(
            new User { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true },
            new User { Email = "john.doe@test.com", FirstName = "Johnny", LastName = "Doe", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ByFirstName_ReturnsUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.AddRange(
            new User { Email = "test1@test.com", FirstName = "Alice", LastName = "Doe", IsActive = true },
            new User { Email = "test2@test.com", FirstName = "Bob", LastName = "Smith", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Alice");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ByLastName_ReturnsUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.AddRange(
            new User { Email = "test1@test.com", FirstName = "John", LastName = "Smith", IsActive = true },
            new User { Email = "test2@test.com", FirstName = "Jane", LastName = "Doe", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Smith");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.Add(new User { Email = "test@test.com", FirstName = "John", LastName = "Doe", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ExcludesInactiveUsers()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new UserRepository(context, _mockLogger.Object);
        context.Users.AddRange(
            new User { Email = "active@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Email = "inactive@test.com", FirstName = "John", LastName = "Smith", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }
}
