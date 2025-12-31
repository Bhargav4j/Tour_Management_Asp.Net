using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services.Tests;

/// <summary>
/// Unit tests for UserService
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new UserService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com" },
            new User { Id = 2, Email = "user2@test.com" }
        };
        var userDtos = new List<UserDto>
        {
            new UserDto { Id = 1, Email = "user1@test.com" },
            new UserDto { Id = 2, Email = "user2@test.com" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com" };
        var userDto = new UserDto { Id = 1, Email = "test@test.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Id = 1, Email = email };
        var userDto = new UserDto { Id = 1, Email = email };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("invalid@test.com", default)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByEmailAsync("invalid@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedUser()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "new@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123"
        };
        var createdUser = new User { Id = 1, Email = "new@test.com", FirstName = "John" };
        var userDto = new UserDto { Id = 1, Email = "new@test.com", FirstName = "John" };

        _mockRepository.Setup(r => r.EmailExistsAsync("new@test.com", default)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), default)).ReturnsAsync(createdUser);
        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("new@test.com", result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "existing@test.com" };
        _mockRepository.Setup(r => r.EmailExistsAsync("existing@test.com", default)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesUser()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "Updated", LastName = "Name" };
        var existingUser = new User { Id = 1, Email = "test@test.com", FirstName = "Old" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.FirstName == "Updated"), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "Updated" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesUser()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingUsers()
    {
        // Arrange
        var searchTerm = "john";
        var users = new List<User> { new User { Id = 1, FirstName = "John" } };
        var userDtos = new List<UserDto> { new UserDto { Id = 1, FirstName = "John" } };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm, default)).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var email = "test@test.com";
        var password = "Password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword, IsActive = true };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync(email, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidPassword_ReturnsFalse()
    {
        // Arrange
        var email = "test@test.com";
        var password = "WrongPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword, IsActive = true };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync(email, password);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", default)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.ValidateLoginAsync("nonexistent@test.com", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Id = 1, Email = email, IsActive = false };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync(email, "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var email = "test@test.com";
        var password = "Password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword, IsActive = true };
        var userDto = new UserDto { Id = 1, Email = email };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.AuthenticateAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var email = "test@test.com";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword, IsActive = true };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, "WrongPassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", default)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync("nonexistent@test.com", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInactiveUser_ReturnsNull()
    {
        // Arrange
        var email = "test@test.com";
        var user = new User { Id = 1, Email = email, IsActive = false };

        _mockRepository.Setup(r => r.GetByEmailAsync(email, default)).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(email, "password");

        // Assert
        Assert.Null(result);
    }
}
