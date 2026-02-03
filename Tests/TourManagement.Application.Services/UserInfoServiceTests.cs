using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserInfoServiceTests
{
    private readonly Mock<IUserInfoRepository> _mockRepository;
    private readonly Mock<ILogger<UserInfoService>> _mockLogger;
    private readonly UserInfoService _service;

    public UserInfoServiceTests()
    {
        _mockRepository = new Mock<IUserInfoRepository>();
        _mockLogger = new Mock<ILogger<UserInfoService>>();
        _service = new UserInfoService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "user1@example.com", FirstName = "John" },
            new UserInfo { Email = "user2@example.com", FirstName = "Jane" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "test@example.com", FirstName = "John" };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", default)).ReturnsAsync(user);

        // Act
        var result = await _service.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("John", result.FirstName);
        _mockRepository.Verify(r => r.GetByEmailAsync("test@example.com", default), Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("invalid@example.com", default)).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.GetByEmailAsync("invalid@example.com");

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByEmailAsync("invalid@example.com", default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var userInfo = new UserInfo { Email = "new@example.com", Password = "password123" };
        var createdUser = new UserInfo { Email = "new@example.com", Password = "hashedpassword" };
        _mockRepository.Setup(r => r.ExistsAsync("new@example.com", default)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), default)).ReturnsAsync(createdUser);

        // Act
        var result = await _service.CreateAsync(userInfo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
        _mockRepository.Verify(r => r.ExistsAsync("new@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ShouldThrowValidationException()
    {
        // Arrange
        var userInfo = new UserInfo { Email = "existing@example.com", Password = "password123" };
        _mockRepository.Setup(r => r.ExistsAsync("existing@example.com", default)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(userInfo));
        _mockRepository.Verify(r => r.ExistsAsync("existing@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), default), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidEmail_ShouldUpdateUser()
    {
        // Arrange
        var existingUser = new UserInfo { Email = "test@example.com", FirstName = "Old Name" };
        var updatedUser = new UserInfo { FirstName = "New Name", LastName = "Doe" };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", default)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync("test@example.com", updatedUser);

        // Assert
        _mockRepository.Verify(r => r.GetByEmailAsync("test@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidEmail_ShouldThrowNotFoundException()
    {
        // Arrange
        var updatedUser = new UserInfo { FirstName = "New Name" };
        _mockRepository.Setup(r => r.GetByEmailAsync("invalid@example.com", default)).ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync("invalid@example.com", updatedUser));
        _mockRepository.Verify(r => r.GetByEmailAsync("invalid@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), default), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithValidEmail_ShouldDeleteUser()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync("test@example.com", default)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync("test@example.com", default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync("test@example.com");

        // Assert
        _mockRepository.Verify(r => r.ExistsAsync("test@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync("test@example.com", default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidEmail_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync("invalid@example.com", default)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync("invalid@example.com"));
        _mockRepository.Verify(r => r.ExistsAsync("invalid@example.com", default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<string>(), default), Times.Never);
    }

    [Fact]
    public async Task ValidateUserAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "test@example.com", Password = "hashedpassword" };
        _mockRepository.Setup(r => r.ValidateUserAsync("test@example.com", It.IsAny<string>(), default)).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateUserAsync("test@example.com", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
        _mockRepository.Verify(r => r.ValidateUserAsync("test@example.com", It.IsAny<string>(), default), Times.Once);
    }

    [Fact]
    public async Task ValidateUserAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.ValidateUserAsync("test@example.com", It.IsAny<string>(), default)).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.ValidateUserAsync("test@example.com", "wrongpassword");

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.ValidateUserAsync("test@example.com", It.IsAny<string>(), default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ShouldReturnMatchingUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "john@example.com", FirstName = "John" },
            new UserInfo { Email = "johnny@example.com", FirstName = "Johnny" }
        };
        _mockRepository.Setup(r => r.SearchAsync("John", default)).ReturnsAsync(users);

        // Act
        var result = await _service.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.SearchAsync("John", default), Times.Once);
    }
}
