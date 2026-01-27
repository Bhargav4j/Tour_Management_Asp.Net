using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public void UserService_Constructor_CreatesInstance()
    {
        // Arrange & Act
        var service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedUserDtos()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com", FirstName = "User1", LastName = "Test1", IsActive = true, CreatedDate = DateTime.UtcNow },
            new User { Id = 2, Email = "user2@test.com", FirstName = "User2", LastName = "Test2", IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Email == "user1@test.com");
        Assert.Contains(result, u => u.Email == "user2@test.com");
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUserDto()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUserDto()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        _mockRepository.Verify(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesUserAndReturnsUserDto()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "newuser@test.com",
            Password = "Password123",
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "+1234567890"
        };

        var createdUser = new User
        {
            Id = 1,
            Email = "newuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            FirstName = "New",
            LastName = "User",
            PhoneNumber = "+1234567890",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("newuser@test.com", result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_HashesPassword()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "test@test.com",
            Password = "PlainPassword",
            FirstName = "Test",
            LastName = "User"
        };

        User capturedUser = null!;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
            .ReturnsAsync((User user, CancellationToken ct) => user);

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedUser);
        Assert.NotEqual("PlainPassword", capturedUser.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("PlainPassword", capturedUser.PasswordHash));
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesUser()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "original@test.com", FirstName = "Original", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new UserUpdateDto
        {
            Email = "updated@test.com",
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+9876543210"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new UserUpdateDto { Email = "updated@test.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsUserDto()
    {
        // Arrange
        var password = "Password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = hashedPassword,
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = hashedPassword,
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.AuthenticateAsync("test@test.com", "WrongPassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.AuthenticateAsync("notfound@test.com", "Password123");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<User>());

        // Act
        await _service.GetAllAsync(cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "test@test.com", FirstName = "Original", LastName = "User", IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new UserUpdateDto { Email = "updated@test.com", FirstName = "Updated", LastName = "User" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        User capturedUser = null!;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, ct) => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(capturedUser);
        Assert.NotNull(capturedUser.ModifiedDate);
    }
}
