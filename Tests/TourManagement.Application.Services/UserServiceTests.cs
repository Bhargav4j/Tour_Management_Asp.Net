using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

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
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UserService(_mockRepository.Object, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com" },
            new User { Id = 2, Email = "user2@test.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenExists()
    {
        var user = new User { Id = 1, Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetUserByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenExists()
    {
        var user = new User { Id = 1, Email = "test@test.com" };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.GetUserByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentNullException_WhenUserIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateUserAsync(null!, "password"));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentException_WhenPasswordIsEmpty()
    {
        var user = new User { Email = "test@test.com" };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(user, ""));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsInvalidOperationException_WhenEmailExists()
    {
        var user = new User { Email = "existing@test.com" };
        _mockRepository.Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateUserAsync(user, "password123"));
    }

    [Fact]
    public async Task UpdateUserAsync_ThrowsArgumentNullException_WhenUserIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateUserAsync(1, null!));
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsInvalidOperationException_WhenUserNotExists()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteUserAsync(999));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WhenUserNotFound()
    {
        _mockRepository.Setup(r => r.GetByEmailAsync("notfound@test.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _service.AuthenticateAsync("notfound@test.com", "password");

        Assert.Null(result);
    }
}
