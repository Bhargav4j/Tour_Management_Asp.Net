using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Services;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1" },
            new User { Id = 2, Username = "user2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithCancellationToken_ShouldPassToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<User>());

        // Act
        await _service.GetAllUsersAsync(cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "testuser" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("testuser", result.Username);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "johndoe" };
        _mockRepository.Setup(r => r.GetByUsernameAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByUsernameAsync("johndoe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("johndoe", result.Username);
        _mockRepository.Verify(r => r.GetByUsernameAsync("johndoe", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var user = new User { Username = "newuser", Email = "new@test.com" };
        _mockRepository.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken ct) => { u.Id = 1; return u; });

        // Act
        var result = await _service.CreateUserAsync(user, "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.True(result.IsActive);
        Assert.NotNull(result.PasswordHash);
        Assert.NotEqual("password123", result.PasswordHash);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateUserAsync(null!, "password"));
    }

    [Fact]
    public async Task CreateUserAsync_WithEmptyPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var user = new User { Username = "testuser" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(user, ""));
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingUsername_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Username = "existinguser" };
        _mockRepository.Setup(r => r.GetByUsernameAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = 1, Username = "existinguser" });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateUserAsync(user, "password"));
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "updateduser" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = 1, Username = "originaluser" });
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updateduser", result.Username);
        Assert.NotNull(result.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateUserAsync(null!));
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Id = 999, Username = "nonexistent" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteUserAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var password = "correctpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("testuser", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("testuser", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var password = "password";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            IsActive = false
        };
        _mockRepository.Setup(r => r.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("testuser", password);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistentUsername_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByUsernameAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync("nonexistent", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchUsersAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Username = "johndoe", Email = "john@test.com" }
        };
        _mockRepository.Setup(r => r.SearchAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.SearchUsersAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        _mockRepository.Verify(r => r.SearchAsync("john", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchUsersAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        _mockRepository.Setup(r => r.SearchAsync("notfound", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _service.SearchUsersAsync("notfound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
