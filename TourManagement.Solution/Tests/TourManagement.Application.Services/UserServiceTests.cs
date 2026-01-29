using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Tests.Application.Services;

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
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new User { Email = "user1@test.com", FirstName = "User1" },
            new User { Email = "user2@test.com", FirstName = "User2" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenRepositoryThrows_ThrowsTourManagementException()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<TourManagementException>(() => _service.GetAllUsersAsync());
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithValidEmail_ReturnsUser()
    {
        var user = new User { Email = "test@example.com", FirstName = "Test" };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetUserByEmailAsync("test@example.com");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WithInvalidEmail_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetUserByEmailAsync("notfound@example.com"));
    }

    [Fact]
    public async Task RegisterUserAsync_WithNewUser_ReturnsUser()
    {
        var user = new User { Email = "newuser@test.com", FirstName = "New" };
        _mockRepository.Setup(r => r.GetByEmailAsync("newuser@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.RegisterUserAsync(user, "password123");

        Assert.NotNull(result);
        Assert.True(result.IsActive);
        Assert.NotEqual(string.Empty, result.PasswordHash);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingEmail_ThrowsEntityAlreadyExistsException()
    {
        var existingUser = new User { Email = "existing@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("existing@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);

        var newUser = new User { Email = "existing@test.com" };
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(() => _service.RegisterUserAsync(newUser, "password"));
    }

    [Fact]
    public async Task UpdateUserAsync_WithExistingUser_UpdatesUser()
    {
        var existingUser = new User { Email = "user@test.com", FirstName = "Old" };
        _mockRepository.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var updatedUser = new User { Email = "user@test.com", FirstName = "Updated" };
        await _service.UpdateUserAsync(updatedUser);

        Assert.NotNull(updatedUser.ModifiedDate);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistingUser_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var user = new User { Email = "notfound@test.com" };
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateUserAsync(user));
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingEmail_DeletesUser()
    {
        _mockRepository.Setup(r => r.ExistsAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync("user@test.com", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        await _service.DeleteUserAsync("user@test.com");

        _mockRepository.Verify(r => r.DeleteAsync("user@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistingEmail_ThrowsEntityNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync("notfound@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteUserAsync("notfound@test.com"));
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithValidCredentials_ReturnsUser()
    {
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = "user@test.com", PasswordHash = hashedPassword };
        _mockRepository.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.AuthenticateUserAsync("user@test.com", password);

        Assert.NotNull(result);
        Assert.Equal("user@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithInvalidPassword_ReturnsNull()
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User { Email = "user@test.com", PasswordHash = hashedPassword };
        _mockRepository.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.AuthenticateUserAsync("user@test.com", "wrongpassword");

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateUserAsync_WithNonExistingUser_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _service.AuthenticateUserAsync("notfound@test.com", "password");

        Assert.Null(result);
    }
}
