using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Domain.Interfaces.Services.Tests;

public class IUserServiceTests
{
    private class TestUserService : IUserService
    {
        private readonly List<User> _users = new();
        private readonly Dictionary<int, string> _passwords = new();

        public Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<User>>(_users.Where(u => u.IsActive).ToList());
        }

        public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Id == id && u.IsActive);
            return Task.FromResult(user);
        }

        public Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Username == username && u.IsActive);
            return Task.FromResult(user);
        }

        public Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            user.Id = _users.Count + 1;
            user.PasswordHash = $"hashed_{password}";
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            _users.Add(user);
            _passwords[user.Id] = password;
            return Task.FromResult(user);
        }

        public Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var existing = _users.Find(u => u.Id == user.Id);
            if (existing != null)
            {
                _users.Remove(existing);
                user.ModifiedDate = DateTime.UtcNow;
                _users.Add(user);
            }
            return Task.FromResult(user);
        }

        public Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Id == id);
            if (user != null)
            {
                user.IsActive = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<User?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Username == username && u.IsActive);
            if (user != null && _passwords.ContainsKey(user.Id) && _passwords[user.Id] == password)
            {
                return Task.FromResult<User?>(user);
            }
            return Task.FromResult<User?>(null);
        }

        public Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var results = _users.Where(u => u.IsActive &&
                (u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))).ToList();
            return Task.FromResult<IEnumerable<User>>(results);
        }
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnActiveUsers()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "user1", Email = "user1@test.com" }, "pass1");
        await service.CreateUserAsync(new User { Username = "user2", Email = "user2@test.com" }, "pass2");

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllUsersAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetAllUsersAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var service = new TestUserService();
        var createdUser = await service.CreateUserAsync(new User { Username = "testuser", Email = "test@test.com" }, "password");

        // Act
        var result = await service.GetUserByIdAsync(createdUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdUser.Id, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var service = new TestUserService();

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetUserByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "johndoe", Email = "john@test.com" }, "password123");

        // Act
        var result = await service.GetUserByUsernameAsync("johndoe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("johndoe", result.Username);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithInvalidUsername_ShouldReturnNull()
    {
        // Arrange
        var service = new TestUserService();

        // Act
        var result = await service.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetUserByUsernameAsync("test", cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateNewUser()
    {
        // Arrange
        var service = new TestUserService();
        var user = new User
        {
            Username = "newuser",
            Email = "newuser@test.com",
            FullName = "New User"
        };

        // Act
        var result = await service.CreateUserAsync(user, "securepassword");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser", result.Username);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
        Assert.StartsWith("hashed_", result.PasswordHash);
    }

    [Fact]
    public async Task CreateUserAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var user = new User { Username = "test", Email = "test@test.com" };
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.CreateUserAsync(user, "password", cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateExistingUser()
    {
        // Arrange
        var service = new TestUserService();
        var user = await service.CreateUserAsync(new User { Username = "original", Email = "original@test.com" }, "password");
        user.Username = "updated";

        // Act
        var result = await service.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated", result.Username);
        Assert.NotNull(result.ModifiedDate);
    }

    [Fact]
    public async Task UpdateUserAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var user = await service.CreateUserAsync(new User { Username = "test", Email = "test@test.com" }, "password");
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.UpdateUserAsync(user, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var service = new TestUserService();
        var user = await service.CreateUserAsync(new User { Username = "todelete", Email = "delete@test.com" }, "password");

        // Act
        var result = await service.DeleteUserAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var service = new TestUserService();

        // Act
        var result = await service.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldSoftDelete()
    {
        // Arrange
        var service = new TestUserService();
        var user = await service.CreateUserAsync(new User { Username = "todelete", Email = "delete@test.com" }, "password");

        // Act
        await service.DeleteUserAsync(user.Id);
        var deletedUser = await service.GetUserByIdAsync(user.Id);

        // Assert
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUserAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.DeleteUserAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "testuser", Email = "test@test.com" }, "correctpassword");

        // Act
        var result = await service.AuthenticateAsync("testuser", "correctpassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "testuser", Email = "test@test.com" }, "correctpassword");

        // Act
        var result = await service.AuthenticateAsync("testuser", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidUsername_ShouldReturnNull()
    {
        // Arrange
        var service = new TestUserService();

        // Act
        var result = await service.AuthenticateAsync("nonexistent", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.AuthenticateAsync("test", "password", cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchUsersAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "johndoe", Email = "john@test.com", FullName = "John Doe" }, "pass1");
        await service.CreateUserAsync(new User { Username = "janedoe", Email = "jane@test.com", FullName = "Jane Doe" }, "pass2");

        // Act
        var result = await service.SearchUsersAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, u => u.Username == "johndoe");
    }

    [Fact]
    public async Task SearchUsersAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        var service = new TestUserService();
        await service.CreateUserAsync(new User { Username = "johndoe", Email = "john@test.com" }, "password");

        // Act
        var result = await service.SearchUsersAsync("notfound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchUsersAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestUserService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.SearchUsersAsync("test", cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
