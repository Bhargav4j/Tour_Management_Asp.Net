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
    public void Constructor_WithNullUserRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockUserRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1" },
            new User { Id = 2, Username = "user2" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "user1" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("user1", result.Username);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "john_doe" };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByUsernameAsync("john_doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john_doe", result.Username);
        _mockUserRepository.Verify(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidUser_CreatesUser()
    {
        // Arrange
        var user = new User { Username = "newuser", Email = "new@example.com" };
        var password = "password123";
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.CreateUserAsync(user, password);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.PasswordHash);
        Assert.NotEmpty(result.PasswordHash);
        Assert.True(result.IsActive);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.CreateUserAsync(null!, "password"));
    }

    [Fact]
    public async Task CreateUserAsync_WithEmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var user = new User { Username = "newuser" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(user, ""));
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User { Username = "existing" };
        var existingUser = new User { Id = 1, Username = "existing" };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("existing", It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(user, "password"));
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidUser_UpdatesUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "user1" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ModifiedDate);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNullUser_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateUserAsync(null!));
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = 999, Username = "nonexistent" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Username = "john_doe", PasswordHash = hashedPassword, IsActive = true };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("john_doe", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john_doe", result.Username);
        _mockUserRepository.Verify(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var correctPassword = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        var user = new User { Id = 1, Username = "john_doe", PasswordHash = hashedPassword, IsActive = true };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("john_doe", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync("nonexistent", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Username = "john_doe", PasswordHash = hashedPassword, IsActive = false };
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("john_doe", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("john_doe", password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchUsersAsync_WithSearchTerm_ReturnsMatchingUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "john_doe" },
            new User { Id = 2, Username = "jane_doe" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync("doe", It.IsAny<CancellationToken>())).ReturnsAsync(users);

        // Act
        var result = await _userService.SearchUsersAsync("doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.SearchAsync("doe", It.IsAny<CancellationToken>()), Times.Once);
    }
}
