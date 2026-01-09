using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for UserService
/// </summary>
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
    public void UserService_Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var service = new UserService(_mockUserRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com" },
            new User { Id = 2, Email = "user2@test.com" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetAllUsersAsync());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByIdAsync(1));
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByEmailAsync("test@test.com"));
    }

    [Fact]
    public async Task CreateUserAsync_WithNewUser_ShouldCreateAndReturnUser()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", PasswordHash = "password123" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(user.IsActive);
        Assert.NotEqual(default(DateTime), user.CreatedDate);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "existing@test.com" };
        var newUser = new User { Email = "existing@test.com", PasswordHash = "password123" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(newUser));
        Assert.Contains("User with email existing@test.com already exists", exception.Message);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", PasswordHash = "password123" };
        var beforeCreate = DateTime.UtcNow;
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.CreateUserAsync(user);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(user.CreatedDate >= beforeCreate && user.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", PasswordHash = "password123", IsActive = false };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.CreateUserAsync(user);

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldHashPassword()
    {
        // Arrange
        var originalPassword = "password123";
        var user = new User { Email = "newuser@test.com", PasswordHash = originalPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.CreateUserAsync(user);

        // Assert
        Assert.NotEqual(originalPassword, user.PasswordHash);
    }

    [Fact]
    public async Task CreateUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", PasswordHash = "password123" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "updated@test.com" };
        var existingUser = new User { Id = 1, Email = "old@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);

        // Assert
        Assert.NotEqual(default(DateTime), user.ModifiedDate);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = 999, Email = "updated@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserAsync(user));
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetModifiedDateToUtcNow()
    {
        // Arrange
        var user = new User { Id = 1, Email = "updated@test.com" };
        var existingUser = new User { Id = 1, Email = "old@test.com" };
        var beforeUpdate = DateTime.UtcNow;
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(user.ModifiedDate);
        Assert.True(user.ModifiedDate >= beforeUpdate && user.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var user = new User { Id = 1, Email = "updated@test.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(1);

        // Assert
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.DeleteUserAsync(999));
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(1));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Id = 1, Email = "test@test.com", PasswordHash = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("test@test.com", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync("invalid@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync("invalid@test.com", "password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User { Id = 1, Email = "test@test.com", PasswordHash = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("test@test.com", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.AuthenticateAsync("test@test.com", "password123"));
    }

    [Fact]
    public async Task SearchUsersAsync_WithValidSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        var searchTerm = "john";
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John" },
            new User { Id = 2, Email = "johnny@test.com", FirstName = "Johnny" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchUsersAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.SearchUsersAsync("test"));
    }
}
