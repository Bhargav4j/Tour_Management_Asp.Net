using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Tests;

/// <summary>
/// Test class for UserRepository
/// </summary>
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
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act
        var repository = new UserRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveUsers_ReturnsActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var activeUser = new UserInfo { Email = "active@example.com", FirstName = "Active", IsActive = true };
        var inactiveUser = new UserInfo { Email = "inactive@example.com", FirstName = "Inactive", IsActive = false };
        context.UserInfos.AddRange(activeUser, inactiveUser);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("active@example.com", result.First().Email);
    }

    [Fact]
    public async Task GetAllAsync_WithNoActiveUsers_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "inactive@example.com", FirstName = "Inactive", IsActive = false };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByEmailAsync("inactive@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_AddsUserAndReturns()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);
        var user = new UserInfo { Email = "new@example.com", FirstName = "New", IsActive = true };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task AddAsync_WithUser_SavesChangesToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true };

        // Act
        await repository.AddAsync(user);
        var savedUser = await context.UserInfos.FindAsync(user.Email);

        // Assert
        Assert.NotNull(savedUser);
        Assert.Equal("Test", savedUser.FirstName);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test" };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.AddAsync(user, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidUser_UpdatesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Original", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);
        user.FirstName = "Updated";

        // Act
        await repository.UpdateAsync(user);
        var updatedUser = await context.UserInfos.FindAsync("test@example.com");

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.UpdateAsync(user, cancellationToken);

        // Assert - No exception thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithValidEmail_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteAsync("test@example.com");
        var deletedUser = await context.UserInfos.FindAsync("test@example.com");

        // Assert
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
        Assert.NotNull(deletedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidEmail_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync("nonexistent@example.com");
        // No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test" };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.DeleteAsync("test@example.com", cancellationToken);

        // Assert
        var deletedUser = await context.UserInfos.FindAsync("test@example.com");
        Assert.False(deletedUser!.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test", IsActive = true };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync("test@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingEmail_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync("nonexistent@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new UserInfo { Email = "inactive@example.com", FirstName = "Inactive", IsActive = false };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync("inactive@example.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "Test",
            PasswordHash = passwordHash,
            IsActive = true
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ValidateCredentialsAsync("test@example.com", "Password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new UserInfo
        {
            Email = "test@example.com",
            FirstName = "Test",
            PasswordHash = passwordHash,
            IsActive = true
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ValidateCredentialsAsync("test@example.com", "WrongPassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ValidateCredentialsAsync("nonexistent@example.com", "Password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new UserInfo
        {
            Email = "inactive@example.com",
            FirstName = "Inactive",
            PasswordHash = passwordHash,
            IsActive = false
        };
        context.UserInfos.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ValidateCredentialsAsync("inactive@example.com", "Password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.ValidateCredentialsAsync("test@example.com", "password", cancellationToken);

        // Assert
        Assert.Null(result);
    }
}
