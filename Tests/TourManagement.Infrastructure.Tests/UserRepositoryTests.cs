using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
    }

    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var repository = new UserRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveUsers_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.AddRange(
            new User { Id = 1, Email = "active@test.com", IsActive = true },
            new User { Id = 2, Email = "inactive@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Id = 1, Email = "test@example.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Id = 1, Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Email = "test@example.com", FirstName = "John", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
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
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldAddAndReturnUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "newuser@example.com", FirstName = "Jane" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("newuser@example.com", result.Email);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUserToDatabase()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var user = new User { Email = "test@example.com", FirstName = "Test" };

        using (var context = new TourManagementDbContext(options))
        {
            var repository = new UserRepository(context, _mockLogger.Object);
            await repository.AddAsync(user);
        }

        // Act & Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedUser = await context.Users.FirstOrDefaultAsync();
            Assert.NotNull(savedUser);
            Assert.Equal("test@example.com", savedUser.Email);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithValidUser_ShouldUpdateUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        User user;

        using (var context = new TourManagementDbContext(options))
        {
            user = new User { Email = "original@example.com", FirstName = "Original" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new UserRepository(context, _mockLogger.Object);
            user.FirstName = "Updated";
            await repository.UpdateAsync(user);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var updatedUser = await context.Users.FirstAsync();
            Assert.Equal("Updated", updatedUser.FirstName);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSetIsActiveFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        User user;

        using (var context = new TourManagementDbContext(options))
        {
            user = new User { Email = "todelete@example.com", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new UserRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(user.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(deletedUser);
            Assert.False(deletedUser.IsActive);
            Assert.NotNull(deletedUser.ModifiedDate);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        User user;

        using (var context = new TourManagementDbContext(options))
        {
            user = new User { Email = "todelete@example.com", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new UserRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(user.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(deletedUser?.ModifiedDate);
        }
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ShouldReturnTrue()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Id = 1, Email = "test@example.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var user = new User { Id = 1, Email = "inactive@test.com", IsActive = false };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingEmail_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.AddRange(
            new User { Email = "john@test.com", FirstName = "John", IsActive = true },
            new User { Email = "jane@test.com", FirstName = "Jane", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("john");

        // Assert
        Assert.Single(result);
        Assert.Equal("john@test.com", result.First().Email);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingFirstName_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.AddRange(
            new User { Email = "user1@test.com", FirstName = "John", IsActive = true },
            new User { Email = "user2@test.com", FirstName = "Jane", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Jane");

        // Assert
        Assert.Single(result);
        Assert.Equal("Jane", result.First().FirstName);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLastName_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.AddRange(
            new User { Email = "user1@test.com", LastName = "Smith", IsActive = true },
            new User { Email = "user2@test.com", LastName = "Doe", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Smith");

        // Assert
        Assert.Single(result);
        Assert.Equal("Smith", result.First().LastName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.Add(new User { Email = "test@example.com", FirstName = "John", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Users.AddRange(
            new User { Email = "active@test.com", FirstName = "John", IsActive = true },
            new User { Email = "inactive@test.com", FirstName = "John", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldRespectToken()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new UserRepository(context, _mockLogger.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}
