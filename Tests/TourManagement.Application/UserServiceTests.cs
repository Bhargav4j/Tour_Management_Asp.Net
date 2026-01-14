using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Application.Tests;

/// <summary>
/// Test class for UserService
/// </summary>
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
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Act
        var service = new UserService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_WithUsers_ReturnsMappedUserDtos()
    {
        // Arrange
        var users = new List<UserInfo> { new UserInfo { Email = "test@example.com", FirstName = "Test" } };
        var userDtos = new List<UserDto> { new UserDto { Email = "test@example.com", FirstName = "Test" } };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("test@example.com", result.First().Email);
    }

    [Fact]
    public async Task GetAllAsync_WithNoUsers_ReturnsEmptyList()
    {
        // Arrange
        var users = new List<UserInfo>();
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
    public async Task GetAllAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var users = new List<UserInfo>();
        _repositoryMock.Setup(r => r.GetAllAsync(cancellationToken)).ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<UserInfo>>())).Returns(new List<UserDto>());

        // Act
        await _service.GetAllAsync(cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsMappedUserDto()
    {
        // Arrange
        var user = new UserInfo { Email = "test@example.com", FirstName = "Test" };
        var userDto = new UserDto { Email = "test@example.com", FirstName = "Test" };
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", cancellationToken)).ReturnsAsync((UserInfo?)null);

        // Act
        await _service.GetByEmailAsync("test@example.com", cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByEmailAsync("test@example.com", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetByEmailAsync("test@example.com"));
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ReturnsCreatedUserDto()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "new@example.com", Password = "Password123", FirstName = "New" };
        var user = new UserInfo { Email = "new@example.com", FirstName = "New" };
        var userDto = new UserDto { Email = "new@example.com", FirstName = "New" };
        _mapperMock.Setup(m => m.Map<UserInfo>(createDto)).Returns(user);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task CreateAsync_HashesPassword()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "new@example.com", Password = "Password123" };
        var user = new UserInfo { Email = "new@example.com" };
        _mapperMock.Setup(m => m.Map<UserInfo>(createDto)).Returns(user);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<UserInfo>())).Returns(new UserDto());

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotEqual("Password123", user.PasswordHash); // Should be hashed
    }

    [Fact]
    public async Task CreateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var createDto = new UserCreateDto { Email = "new@example.com", Password = "Password123" };
        var user = new UserInfo { Email = "new@example.com" };
        _mapperMock.Setup(m => m.Map<UserInfo>(createDto)).Returns(user);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), cancellationToken)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<UserInfo>())).Returns(new UserDto());

        // Act
        await _service.CreateAsync(createDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "new@example.com", Password = "Password123" };
        var user = new UserInfo { Email = "new@example.com" };
        _mapperMock.Setup(m => m.Map<UserInfo>(createDto)).Returns(user);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_WithValidEmailAndDto_UpdatesUser()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "Updated" };
        var existingUser = new UserInfo { Email = "test@example.com", FirstName = "Original" };
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mapperMock.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);
        _repositoryMock.Setup(r => r.UpdateAsync(existingUser, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync("test@example.com", updateDto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidEmail_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "Updated" };
        _repositoryMock.Setup(r => r.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync("nonexistent@example.com", updateDto));
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var updateDto = new UserUpdateDto { FirstName = "Updated" };
        var existingUser = new UserInfo { Email = "test@example.com", FirstName = "Original" };
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", cancellationToken)).ReturnsAsync(existingUser);
        _mapperMock.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);
        _repositoryMock.Setup(r => r.UpdateAsync(existingUser, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync("test@example.com", updateDto, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByEmailAsync("test@example.com", cancellationToken), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "Updated" };
        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync("test@example.com", updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidEmail_CallsRepositoryDelete()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync("test@example.com", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync("test@example.com");

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync("test@example.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _repositoryMock.Setup(r => r.DeleteAsync("test@example.com", cancellationToken)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync("test@example.com", cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync("test@example.com", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync("test@example.com"));
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new UserInfo { Email = "test@example.com", PasswordHash = passwordHash };
        var userDto = new UserDto { Email = "test@example.com" };
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.ValidateLoginAsync("test@example.com", "Password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new UserInfo { Email = "test@example.com", PasswordHash = passwordHash };
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync("test@example.com", "WrongPassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.ValidateLoginAsync("nonexistent@example.com", "Password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", cancellationToken)).ReturnsAsync((UserInfo?)null);

        // Act
        await _service.ValidateLoginAsync("test@example.com", "Password123", cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.GetByEmailAsync("test@example.com", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ValidateLoginAsync_WhenRepositoryThrows_RethrowsException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.ValidateLoginAsync("test@example.com", "Password123"));
    }
}
