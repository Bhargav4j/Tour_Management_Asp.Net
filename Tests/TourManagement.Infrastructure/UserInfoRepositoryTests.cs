using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class UserInfoRepositoryTests
{
    private readonly Mock<ILogger<UserInfoRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbContextOptions;

    public UserInfoRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserInfoRepository>>();
        _dbContextOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void UserInfoRepository_Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void UserInfoRepository_Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        context.UserInfos.AddRange(
            new UserInfo { Email = "user1@test.com", IsActive = true },
            new UserInfo { Email = "user2@test.com", IsActive = true },
            new UserInfo { Email = "user3@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveUsersExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "test@example.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "inactive@test.com", IsActive = false };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "test@example.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenUserIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "inactive@test.com", IsActive = false };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);
        var user = new UserInfo { Email = "newuser@test.com", FirstName = "John" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser@test.com", result.Email);

        var savedUser = await context.UserInfos.FindAsync(result.Id);
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserInDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "old@test.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.Email = "new@test.com";
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.UserInfos.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("new@test.com", updatedUser.Email);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "delete@test.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();
        var userId = user.Id;

        // Act
        await repository.DeleteAsync(userId);

        // Assert
        var deletedUser = await context.UserInfos.FindAsync(userId);
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
        Assert.NotNull(deletedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(999);

        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "exists@test.com", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenUserIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo { Email = "inactive@test.com", IsActive = false };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        context.UserInfos.AddRange(
            new UserInfo { Email = "john@test.com", FirstName = "John", IsActive = true },
            new UserInfo { Email = "jane@test.com", FirstName = "Jane", IsActive = true },
            new UserInfo { Email = "johnny@test.com", FirstName = "Johnny", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllUsers_WhenSearchTermIsEmpty()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        context.UserInfos.AddRange(
            new UserInfo { Email = "user1@test.com", IsActive = true },
            new UserInfo { Email = "user2@test.com", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmptyList_WhenNoMatchFound()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        context.UserInfos.Add(new UserInfo { Email = "test@test.com", FirstName = "Test", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsTrue_WhenCredentialsAreValid()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo
        {
            Email = "test@example.com",
            Password = "password123",
            IsActive = true
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ValidateCredentialsAsync("test@example.com", "password123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsFalse_WhenPasswordIsIncorrect()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo
        {
            Email = "test@example.com",
            Password = "correctpassword",
            IsActive = true
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ValidateCredentialsAsync("test@example.com", "wrongpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ValidateCredentialsAsync("nonexistent@example.com", "password123");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsFalse_WhenUserIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        var user = new UserInfo
        {
            Email = "inactive@example.com",
            Password = "password123",
            IsActive = false
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ValidateCredentialsAsync("inactive@example.com", "password123");

        // Assert
        Assert.False(result);
    }
}
