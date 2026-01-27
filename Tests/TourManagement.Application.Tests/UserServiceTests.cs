using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests;

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
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new UserService(_mockUserRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenCalled_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com" },
            new User { Id = 2, Email = "user2@test.com" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
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
        var expectedUser = new User { Id = 1, Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
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
        var email = "test@example.com";
        var expectedUser = new User { Id = 1, Email = email };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        _mockUserRepository.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

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
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByEmailAsync("test@example.com"));
    }

    [Fact]
    public async Task RegisterUserAsync_WithNewUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var newUser = new User { Email = "newuser@example.com", FirstName = "John" };
        var password = "password123";
        var createdUser = new User { Id = 1, Email = "newuser@example.com", FirstName = "John" };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(newUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userService.RegisterUserAsync(newUser, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(newUser.IsActive);
        Assert.NotEqual(default(DateTime), newUser.CreatedDate);
        Assert.NotEmpty(newUser.PasswordHash);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "existing@example.com" };
        var newUser = new User { Email = "existing@example.com" };
        var password = "password123";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(newUser.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.RegisterUserAsync(newUser, password));
        Assert.Contains("User with email existing@example.com already exists", exception.Message);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldHashPassword()
    {
        // Arrange
        var newUser = new User { Email = "test@example.com" };
        var password = "plainPassword";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newUser);

        // Act
        await _userService.RegisterUserAsync(newUser, password);

        // Assert
        Assert.NotEmpty(newUser.PasswordHash);
        Assert.NotEqual(password, newUser.PasswordHash);
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var newUser = new User { Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.RegisterUserAsync(newUser, "password"));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "test@example.com";
        var correctPassword = "password123";
        var wrongPassword = "wrongpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword };

        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, wrongPassword);

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
        await Assert.ThrowsAsync<Exception>(() => _userService.AuthenticateAsync("test@example.com", "password"));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_ShouldUpdateSuccessfully()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "old@example.com" };
        var updatedUser = new User { Id = 1, Email = "new@example.com" };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotEqual(default(DateTime), updatedUser.ModifiedDate);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var updatedUser = new User { Id = 999, Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUserAsync(updatedUser));
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "test@example.com" };
        var updatedUser = new User { Id = 1, Email = "updated@example.com" };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotNull(updatedUser.ModifiedDate);
        Assert.NotEqual(default(DateTime), updatedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ShouldDeleteSuccessfully()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(1);

        // Assert
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.DeleteUserAsync(999));
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(1));
    }

    [Fact]
    public async Task SearchUsersAsync_WithSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        var searchTerm = "john";
        var expectedUsers = new List<User>
        {
            new User { Id = 1, Email = "john@example.com", FirstName = "John" },
            new User { Id = 2, Email = "johnny@example.com", FirstName = "Johnny" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchUsersAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var searchTerm = "nonexistent";
        _mockUserRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
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

    [Fact]
    public async Task GetAllUsersAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _mockUserRepository.Setup(r => r.GetAllAsync(cancellationToken))
            .ReturnsAsync(new List<User>());

        // Act
        await _userService.GetAllUsersAsync(cancellationToken);

        // Assert
        _mockUserRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }
}
