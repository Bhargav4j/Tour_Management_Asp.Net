using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserInfoRepositoryTests
{
    private readonly Mock<ILogger<UserInfoRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public UserInfoRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserInfoRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.UserInfos.AddRange(
            new UserInfo { Email = "user1@test.com", FirstName = "John", IsActive = true, CreatedBy = "System" },
            new UserInfo { Email = "user2@test.com", FirstName = "Jane", IsActive = true, CreatedBy = "System" },
            new UserInfo { Email = "user3@test.com", FirstName = "Bob", IsActive = false, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("invalid@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserAndReturnIt()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserInfoRepository(context, _mockLogger.Object);
        var user = new UserInfo { Email = "new@example.com", FirstName = "New User", IsActive = true, CreatedBy = "System" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
        Assert.Equal("New User", result.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Old Name", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);
        user.FirstName = "Updated Name";

        // Act
        await repository.UpdateAsync(user);

        // Assert
        var updatedUser = await context.UserInfos.FindAsync("test@example.com");
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated Name", updatedUser.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync("test@example.com");

        // Assert
        var deletedUser = await context.UserInfos.FindAsync("test@example.com");
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync("test@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync("invalid@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateUserAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", Password = "hashedpass", FirstName = "Test", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ValidateUserAsync("test@example.com", "hashedpass");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ValidateUserAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", Password = "hashedpass", IsActive = true, CreatedBy = "System" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ValidateUserAsync("test@example.com", "wrongpass");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.UserInfos.AddRange(
            new UserInfo { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true, CreatedBy = "System" },
            new UserInfo { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, u => u.FirstName == "John");
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ShouldReturnAllUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.UserInfos.AddRange(
            new UserInfo { Email = "user1@test.com", FirstName = "User1", IsActive = true, CreatedBy = "System" },
            new UserInfo { Email = "user2@test.com", FirstName = "User2", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new UserInfoRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
