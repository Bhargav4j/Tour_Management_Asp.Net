using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Application.Services.Tests;

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
    public async Task GetAllAsync_ReturnsUserDtos()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1, Email = "test@test.com" } };
        var userDtos = new List<UserDto> { new UserDto { Id = 1, Email = "test@test.com" } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUserDto_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com" };
        var userDto = new UserDto { Id = 1, Email = "test@test.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenEmailExists()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "test@test.com" };
        _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    [Fact]
    public async Task ValidateLoginAsync_ReturnsTrue_WhenCredentialsValid()
    {
        // Arrange
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = "test@test.com", PasswordHash = hashedPassword, IsActive = true };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync("test@test.com", password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_ReturnsFalse_WhenUserNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.ValidateLoginAsync("test@test.com", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1, FirstName = "John" } };
        var userDtos = new List<UserDto> { new UserDto { Id = 1, FirstName = "John" } };

        _mockRepository.Setup(r => r.SearchAsync("John", It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
