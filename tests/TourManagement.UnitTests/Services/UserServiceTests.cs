using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

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
    public async Task GetAllAsync_ShouldReturnUserDtos()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1, Email = "user@test.com" } };
        var userDtos = new List<UserDto> { new UserDto { Id = 1, Email = "user@test.com" } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("user@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        var userDto = new UserDto { Id = 1, Email = "test@example.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserDto_WhenEmailExists()
    {
        // Arrange
        var user = new User { Id = 1, Email = "unique@test.com" };
        var userDto = new UserDto { Id = 1, Email = "unique@test.com" };

        _mockRepository.Setup(r => r.GetByEmailAsync("unique@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.GetByEmailAsync("unique@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("unique@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_AndReturnUserDto()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "new@user.com", Password = "password123" };
        var user = new User { Email = "new@user.com" };
        var createdUser = new User { Id = 1, Email = "new@user.com", PasswordHash = "hashed" };
        var userDto = new UserDto { Id = 1, Email = "new@user.com" };

        _mockMapper.Setup(m => m.Map<User>(createDto)).Returns(user);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);
        _mockMapper.Setup(m => m.Map<UserDto>(createdUser)).Returns(userDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new@user.com", result.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "UpdatedName" };
        var existingUser = new User { Id = 1, FirstName = "OldName" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockMapper.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(existingUser, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var updateDto = new UserUpdateDto { FirstName = "UpdatedName" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnUserDto_WhenCredentialsAreValid()
    {
        // Arrange
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Id = 1, Email = "test@example.com", PasswordHash = hashedPassword };
        var userDto = new UserDto { Id = 1, Email = "test@example.com" };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _service.ValidateLoginAsync("test@example.com", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("nonexistent@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        // Act
        var result = await _service.ValidateLoginAsync("nonexistent@test.com", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateLoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User { Id = 1, Email = "test@example.com", PasswordHash = hashedPassword };

        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync("test@example.com", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockRepository.Object, _mockMapper.Object, null!));
    }
}
