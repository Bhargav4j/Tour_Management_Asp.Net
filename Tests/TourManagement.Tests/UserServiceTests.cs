using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
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
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new UserService(_mockUserRepository.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockUserRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User { Email = "user1@test.com", FirstName = "John" },
            new User { Email = "user2@test.com", FirstName = "Jane" }
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
    public async Task GetAllUsersAsync_WithEmptyResult_ShouldReturnEmptyCollection()
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
    public async Task GetUserByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new User { Email = email, FirstName = "John" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal("John", result.FirstName);
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
    public async Task RegisterUserAsync_WithNewUser_ShouldRegisterAndReturnUser()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", FirstName = "John" };
        var password = "Password123!";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.RegisterUserAsync(user, password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.NotEqual(default(DateTime), user.CreatedDate);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldHashPassword()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", FirstName = "John" };
        var password = "Password123!";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.RegisterUserAsync(user, password);

        // Assert
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEqual(password, user.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash));
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldSetCreatedDateToUtcNow()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com" };
        var password = "Password123!";
        var beforeRegister = DateTime.UtcNow;
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.RegisterUserAsync(user, password);
        var afterRegister = DateTime.UtcNow;

        // Assert
        Assert.True(user.CreatedDate >= beforeRegister);
        Assert.True(user.CreatedDate <= afterRegister);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com", IsActive = false };
        var password = "Password123!";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userService.RegisterUserAsync(user, password);

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Email = "existing@test.com" };
        var password = "Password123!";
        var existingUser = new User { Email = "existing@test.com" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _userService.RegisterUserAsync(user, password));
    }

    [Fact]
    public async Task RegisterUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var user = new User { Email = "newuser@test.com" };
        var password = "Password123!";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.RegisterUserAsync(user, password));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var user = new User { Email = "test@example.com", FirstName = "UpdatedName" };
        var existingUser = new User { Email = "test@example.com", FirstName = "OldName" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);

        // Assert
        Assert.NotEqual(default(DateTime), user.ModifiedDate);
        _mockUserRepository.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldSetModifiedDateToUtcNow()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        var existingUser = new User { Email = "test@example.com" };
        var beforeUpdate = DateTime.UtcNow;
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(user.ModifiedDate);
        Assert.True(user.ModifiedDate >= beforeUpdate);
        Assert.True(user.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Email = "nonexistent@test.com" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingEmail_ShouldDeleteUser()
    {
        // Arrange
        var email = "test@example.com";
        _mockUserRepository.Setup(r => r.ExistsAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(email, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(email);

        // Assert
        _mockUserRepository.Verify(r => r.DeleteAsync(email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var email = "nonexistent@test.com";
        _mockUserRepository.Setup(r => r.ExistsAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.DeleteUserAsync(email));
    }

    [Fact]
    public async Task DeleteUserAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var email = "test@example.com";
        _mockUserRepository.Setup(r => r.ExistsAsync(email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUserAsync(email));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = email, PasswordHash = hashedPassword };
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
        var password = "Password123!";
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
        var password = "WrongPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!");
        var user = new User { Email = email, PasswordHash = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.AuthenticateAsync(email, password));
    }

    [Fact]
    public async Task AuthenticateAsync_WithEmptyPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "test@example.com";
        var password = "";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
        var user = new User { Email = email, PasswordHash = hashedPassword };
        _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

        // Assert
        Assert.Null(result);
    }
}
