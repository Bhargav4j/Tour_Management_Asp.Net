using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests.Services;

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
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockUserRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com", FirstName = "John" },
            new User { Id = 2, Email = "user2@test.com", FirstName = "Jane" }
        };
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenExceptionThrown_ShouldLogErrorAndRethrow()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "John" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "John" };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        _mockUserRepository.Verify(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNewUser_ShouldCreateAndReturnUser()
    {
        // Arrange
        var user = new User
        {
            Email = "new@test.com",
            FirstName = "John",
            LastName = "Doe"
        };
        _mockUserRepository.Setup(r => r.EmailExistsAsync("new@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.CreateAsync(user, "password123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotNull(result.PasswordHash);
        Assert.NotEmpty(result.PasswordHash);
        Assert.Equal(user.Email, result.CreatedBy);
        _mockUserRepository.Verify(r => r.EmailExistsAsync("new@test.com", It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { Email = "existing@test.com" };
        _mockUserRepository.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _userService.CreateAsync(user, "password123"));
        Assert.Contains("already exists", exception.Message);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var existingUser = new User
        {
            Id = 1,
            Email = "test@test.com",
            FirstName = "Old",
            LastName = "Name"
        };
        var updatedUser = new User
        {
            FirstName = "New",
            LastName = "Name",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 St",
            City = "City",
            State = "ST",
            Email = "test@test.com"
        };
        _mockUserRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateAsync(1, updatedUser);

        // Assert
        Assert.Equal("New", existingUser.FirstName);
        Assert.Equal("Name", existingUser.LastName);
        Assert.Equal("Male", existingUser.Gender);
        Assert.NotNull(existingUser.ModifiedDate);
        Assert.Equal("test@test.com", existingUser.ModifiedBy);
        _mockUserRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = new User { FirstName = "John" };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _userService.UpdateAsync(999, user));
        Assert.Contains("not found", exception.Message);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteAsync(1);

        // Assert
        _mockUserRepository.Verify(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockUserRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _userService.DeleteAsync(999));
        Assert.Contains("not found", exception.Message);
        _mockUserRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash
        };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("test@test.com", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        _mockUserRepository.Verify(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync("notfound@test.com", "password123");

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash
        };
        _mockUserRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync("test@test.com", "wrongpassword");

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John" },
            new User { Id = 2, Email = "johnny@test.com", FirstName = "Johnny" }
        };
        _mockUserRepository.Setup(r => r.SearchAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userService.SearchAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.SearchAsync("john", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.SearchAsync("xyz");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockUserRepository.Verify(r => r.SearchAsync("xyz", It.IsAny<CancellationToken>()), Times.Once);
    }
}
