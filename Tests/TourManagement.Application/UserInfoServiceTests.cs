using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Tests;

public class UserInfoServiceTests
{
    private readonly Mock<IUserInfoRepository> _mockUserInfoRepository;
    private readonly Mock<ILogger<UserInfoService>> _mockLogger;
    private readonly UserInfoService _userInfoService;

    public UserInfoServiceTests()
    {
        _mockUserInfoRepository = new Mock<IUserInfoRepository>();
        _mockLogger = new Mock<ILogger<UserInfoService>>();
        _userInfoService = new UserInfoService(_mockUserInfoRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void UserInfoService_Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(null!, _mockLogger.Object));
    }

    [Fact]
    public void UserInfoService_Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserInfoService(_mockUserInfoRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Id = 1, Email = "user1@test.com" },
            new UserInfo { Id = 2, Email = "user2@test.com" }
        };
        _mockUserInfoRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userInfoService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserInfoRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockUserInfoRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfo>());

        // Act
        var result = await _userInfoService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "test@example.com" };
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userInfoService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _userInfoService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "test@example.com" };
        _mockUserInfoRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userInfoService.GetUserByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserInfoRepository.Setup(r => r.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _userInfoService.GetUserByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_CreatesAndReturnsUser()
    {
        // Arrange
        var user = new UserInfo { Email = "newuser@test.com", FirstName = "John" };
        var createdUser = new UserInfo { Id = 1, Email = "newuser@test.com", FirstName = "John" };
        _mockUserInfoRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userInfoService.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("newuser@test.com", result.Email);
        Assert.True(user.IsActive);
        _mockUserInfoRepository.Verify(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_SetsCreatedDate()
    {
        // Arrange
        var user = new UserInfo { Email = "test@test.com" };
        var beforeCreate = DateTime.UtcNow;
        _mockUserInfoRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userInfoService.CreateUserAsync(user);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.True(user.CreatedDate >= beforeCreate && user.CreatedDate <= afterCreate);
    }

    [Fact]
    public async Task CreateUserAsync_SetsIsActiveToTrue()
    {
        // Arrange
        var user = new UserInfo { Email = "test@test.com", IsActive = false };
        _mockUserInfoRepository.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _userInfoService.CreateUserAsync(user);

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new UserInfo { Id = 1, Email = "old@test.com" };
        var updatedUser = new UserInfo { Id = 1, Email = "updated@test.com" };
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserInfoRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userInfoService.UpdateUserAsync(updatedUser);

        // Assert
        Assert.NotNull(updatedUser.ModifiedDate);
        _mockUserInfoRepository.Verify(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var user = new UserInfo { Id = 999, Email = "nonexistent@test.com" };
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _userInfoService.UpdateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_SetsModifiedDate()
    {
        // Arrange
        var existingUser = new UserInfo { Id = 1, Email = "test@test.com" };
        var user = new UserInfo { Id = 1, Email = "updated@test.com" };
        var beforeUpdate = DateTime.UtcNow;
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mockUserInfoRepository.Setup(r => r.UpdateAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userInfoService.UpdateUserAsync(user);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(user.ModifiedDate);
        Assert.True(user.ModifiedDate >= beforeUpdate && user.ModifiedDate <= afterUpdate);
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesUser_WhenUserExists()
    {
        // Arrange
        var user = new UserInfo { Id = 1, Email = "delete@test.com" };
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockUserInfoRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userInfoService.DeleteUserAsync(1);

        // Assert
        _mockUserInfoRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserInfoRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _userInfoService.DeleteUserAsync(999));
    }

    [Fact]
    public async Task SearchUsersAsync_ReturnsMatchingUsers()
    {
        // Arrange
        var searchTerm = "John";
        var users = new List<UserInfo>
        {
            new UserInfo { Id = 1, FirstName = "John", Email = "john1@test.com" },
            new UserInfo { Id = 2, FirstName = "Johnny", Email = "john2@test.com" }
        };
        _mockUserInfoRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _userInfoService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockUserInfoRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchUsersAsync_ReturnsEmptyList_WhenNoMatchFound()
    {
        // Arrange
        var searchTerm = "NonExistent";
        _mockUserInfoRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserInfo>());

        // Act
        var result = await _userInfoService.SearchUsersAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsTrue_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        _mockUserInfoRepository.Setup(r => r.ValidateCredentialsAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userInfoService.ValidateCredentialsAsync(email, password);

        // Assert
        Assert.True(result);
        _mockUserInfoRepository.Verify(r => r.ValidateCredentialsAsync(email, password, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_ReturnsFalse_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        _mockUserInfoRepository.Setup(r => r.ValidateCredentialsAsync(email, password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _userInfoService.ValidateCredentialsAsync(email, password);

        // Assert
        Assert.False(result);
    }
}
