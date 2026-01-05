using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _service = new UserService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act & Assert
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUserDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, Email = "user1@test.com" },
            new User { UserId = 2, Email = "user2@test.com" }
        };
        var userDtos = new List<UserDto>
        {
            new UserDto { UserId = 1, Email = "user1@test.com" },
            new UserDto { UserId = 2, Email = "user2@test.com" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        var users = new List<User>();
        var userDtos = new List<UserDto>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUserDto()
    {
        // Arrange
        var user = new User { UserId = 1, Email = "test@test.com" };
        var userDto = new UserDto { UserId = 1, Email = "test@test.com" };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnUserDto()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "new@test.com",
            Password = "password123",
            FirstName = "John",
            LastName = "Doe"
        };
        var user = new User { Email = "new@test.com", FirstName = "John", LastName = "Doe" };
        var createdUser = new User { UserId = 1, Email = "new@test.com", FirstName = "John", LastName = "Doe" };
        var userDto = new UserDto { UserId = 1, Email = "new@test.com", FirstName = "John", LastName = "Doe" };

        _repositoryMock.Setup(r => r.EmailExistsAsync(registerDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<User>(registerDto)).Returns(user);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);
        _mapperMock.Setup(m => m.Map<UserDto>(createdUser)).Returns(userDto);

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal("new@test.com", result.Email);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "existing@test.com",
            Password = "password123"
        };

        _repositoryMock.Setup(r => r.EmailExistsAsync(registerDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUserDto()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "test@test.com",
            Password = "password123"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash
        };
        var userDto = new UserDto { UserId = 1, Email = "test@test.com" };

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "invalid@test.com",
            Password = "password123"
        };

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "test@test.com",
            Password = "wrongpassword"
        };
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            PasswordHash = passwordHash
        };

        _repositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync(loginDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "Updated",
            LastName = "User"
        };
        var existingUser = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FirstName = "Original",
            LastName = "User"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mapperMock.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "Updated",
            LastName = "User"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldStillCallRepository()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(999);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }
}
