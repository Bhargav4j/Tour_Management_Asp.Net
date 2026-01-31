using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Domain.Interfaces.Repositories.Tests;

public class IUserRepositoryTests
{
    private class TestUserRepository : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<User>>(_users);
        }

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Username == username);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            user.Id = _users.Count + 1;
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var existing = _users.Find(u => u.Id == user.Id);
            if (existing != null)
            {
                _users.Remove(existing);
                _users.Add(user);
            }
            return Task.FromResult(user);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = _users.Find(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Exists(u => u.Id == id));
        }

        public Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var results = _users.FindAll(u =>
                u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
            return Task.FromResult<IEnumerable<User>>(results);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var repository = new TestUserRepository();
        await repository.AddAsync(new User { Username = "user1", Email = "user1@test.com" });
        await repository.AddAsync(new User { Username = "user2", Email = "user2@test.com" });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<User>)result).Count);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var repository = new TestUserRepository();
        var addedUser = await repository.AddAsync(new User { Username = "testuser", Email = "test@test.com" });

        // Act
        var result = await repository.GetByIdAsync(addedUser.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedUser.Id, result.Id);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var repository = new TestUserRepository();

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var repository = new TestUserRepository();
        await repository.AddAsync(new User { Username = "johndoe", Email = "john@test.com" });

        // Act
        var result = await repository.GetByUsernameAsync("johndoe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("johndoe", result.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithInvalidUsername_ShouldReturnNull()
    {
        // Arrange
        var repository = new TestUserRepository();

        // Act
        var result = await repository.GetByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByUsernameAsync("test", cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var repository = new TestUserRepository();
        await repository.AddAsync(new User { Username = "johndoe", Email = "john@test.com" });

        // Act
        var result = await repository.GetByEmailAsync("john@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var repository = new TestUserRepository();

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com", cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = new User { Username = "newuser", Email = "new@test.com" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("newuser", result.Username);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = new User { Username = "newuser", Email = "new@test.com" };
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.AddAsync(user, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingUser()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = await repository.AddAsync(new User { Username = "original", Email = "original@test.com" });
        user.Username = "updated";

        // Act
        var result = await repository.UpdateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("updated", result.Username);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = await repository.AddAsync(new User { Username = "test", Email = "test@test.com" });
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.UpdateAsync(user, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = await repository.AddAsync(new User { Username = "todelete", Email = "delete@test.com" });

        // Act
        var result = await repository.DeleteAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestUserRepository();

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.DeleteAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestUserRepository();
        var user = await repository.AddAsync(new User { Username = "test", Email = "test@test.com" });

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestUserRepository();

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.ExistsAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var repository = new TestUserRepository();
        await repository.AddAsync(new User { Username = "johndoe", Email = "john@test.com", FullName = "John Doe" });
        await repository.AddAsync(new User { Username = "janedoe", Email = "jane@test.com", FullName = "Jane Doe" });

        // Act
        var result = await repository.SearchAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        var repository = new TestUserRepository();
        await repository.AddAsync(new User { Username = "johndoe", Email = "john@test.com" });

        // Act
        var result = await repository.SearchAsync("notfound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestUserRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.SearchAsync("test", cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
