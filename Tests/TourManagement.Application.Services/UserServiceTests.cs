using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockUserRepository.Object, null));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@example.com" },
            new User { Id = 2, Email = "user2@example.com" }
        };
        _mockUserRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetAllUsersAsync());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Email = "test@example.com" };
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@example.com", result.Email);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByIdAsync(1));
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var user = new User { Id = 1, Email = email };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        _mockUserRepository.Verify(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByEmailAsync("test@example.com"));
    }

    [Fact]
    public async Task CreateUserAsync_WithNewEmail_CreatesUser()
    {
        // Arrange
        var user = new User { Email = "newuser@example.com", FirstName = "John" };
        var password = "Password123!";
        var createdUser = new User { Id = 1, Email = "newuser@example.com", FirstName = "John" };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(user, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.NotEqual(default(DateTime), user.CreatedDate);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var email = "existing@example.com";
        var user = new User { Email = email };
        var existingUser = new User { Id = 1, Email = email };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(user, "Password123!"));
    }

    [Fact]
    public async Task CreateUserAsync_HashesPassword()
    {
        // Arrange
        var user = new User { Email = "newuser@example.com" };
        var password = "Password123!";
        var createdUser = new User { Id = 1, Email = "newuser@example.com" };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);

        // Act
        await _userService.CreateUserAsync(user, password);

        // Assert
        Assert.NotNull(user.PasswordHash);
        Assert.NotEqual(password, user.PasswordHash);
        Assert.True(user.PasswordHash.StartsWith("$2"));
    }

    [Fact]
    public async Task CreateUserAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var user = new User { Email = "newuser@example.com" };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(user, "Password123!"));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_UpdatesSuccessfully()
    {
        // Arrange
        var userId = 1;
        var existingUser = new User { Id = userId, Email = "old@example.com" };
        var updatedUser = new User { Id = userId, Email = "new@example.com" };
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotEqual(default(DateTime), updatedUser.ModifiedDate);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistingUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = 999, Email = "nonexistent@example.com" };
        _mockUserRepository.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_SetsModifiedDate()
    {
        // Arrange
        var user = new User { Id = 1, Email = "updated@example.com" };
        var existingUser = new User { Id = 1, Email = "old@example.com" };
        _mockUserRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(user.ModifiedDate);
        Assert.NotEqual(default(DateTime), user.ModifiedDate);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var user = new User { Id = 1, Email = "updated@example.com" };
        var existingUser = new User { Id = 1, Email = "old@example.com" };
        _mockUserRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingId_DeletesSuccessfully()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.ExistsAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockUserRepository.Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _mockUserRepository.Verify(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistingId_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.ExistsAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.DeleteUserAsync(userId));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.ExistsAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockUserRepository.Setup(x => x.DeleteAsync(userId, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(userId));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Email = email, PasswordHash = passwordHash };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Password123!";
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "test@example.com";
        var correctPassword = "Password123!";
        var incorrectPassword = "WrongPassword";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        var user = new User { Id = 1, Email = email, PasswordHash = passwordHash };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, incorrectPassword);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.AuthenticateAsync("test@example.com", "Password123!"));
    }

    [Fact]
    public async Task SearchUsersAsync_WithSearchTerm_ReturnsUsers()
    {
        // Arrange
        var searchTerm = "john";
        var users = new List<User>
        {
            new User { Id = 1, Email = "john.doe@example.com", FirstName = "John" },
            new User { Id = 2, Email = "john.smith@example.com", FirstName = "John" }
        };
        _mockUserRepository.Setup(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchUsersAsync_WhenRepositoryThrows_ThrowsException()
    {
        // Arrange
        var searchTerm = "john";
        _mockUserRepository.Setup(x => x.SearchAsync(searchTerm, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.SearchUsersAsync(searchTerm));
    }
}
