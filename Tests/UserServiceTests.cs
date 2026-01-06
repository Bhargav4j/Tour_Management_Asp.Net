using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class UserServiceTests
{
    private class MockUserRepository : IUserRepository
    {
        public List<User> Users { get; set; } = new List<User>();
        public bool ThrowException { get; set; }

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Users.AsEnumerable());
        }

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            entity.Id = Users.Count + 1;
            Users.Add(entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var existing = Users.FirstOrDefault(u => u.Id == entity.Id);
            if (existing != null)
            {
                var index = Users.IndexOf(existing);
                Users[index] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                Users.Remove(user);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Users.Any(u => u.Id == id));
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Users.Any(u => u.Email == email));
        }

        public Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var results = Users.Where(u => u.Email.Contains(searchTerm) || u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm));
            return Task.FromResult(results.AsEnumerable());
        }
    }

    private class MockLogger : ILogger<UserService>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var repository = new MockUserRepository();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(repository, null!));
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "user1@test.com" });
        repository.Users.Add(new User { Id = 2, Email = "user2@test.com" });
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsers()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllUsersAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockUserRepository { ThrowException = true };
        var service = new UserService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetAllUsersAsync());
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "test@test.com" });
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "test@test.com" });
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetUserByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.GetUserByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_AddsUser()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "new@test.com", FirstName = "John" };

        // Act
        var result = await service.CreateUserAsync(user, "password123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.True(result.IsActive);
        Assert.NotEmpty(result.PasswordHash);
        Assert.Single(repository.Users);
    }

    [Fact]
    public async Task CreateUserAsync_HashesPassword()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "new@test.com" };
        var password = "password123";

        // Act
        var result = await service.CreateUserAsync(user, password);

        // Assert
        Assert.NotEqual(password, result.PasswordHash);
        Assert.NotEmpty(result.PasswordHash);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsInvalidOperationException_WhenEmailExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "existing@test.com" });
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "existing@test.com" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateUserAsync(user, "password"));
    }

    [Fact]
    public async Task CreateUserAsync_SetsCreatedDate()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "new@test.com" };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await service.CreateUserAsync(user, "password");
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(result.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUser_WhenExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "original@test.com" });
        var service = new UserService(repository, new MockLogger());
        var updatedUser = new User { Id = 1, Email = "updated@test.com" };

        // Act
        await service.UpdateUserAsync(updatedUser);

        // Assert
        var user = repository.Users.First();
        Assert.Equal("updated@test.com", user.Email);
        Assert.NotNull(user.ModifiedDate);
    }

    [Fact]
    public async Task UpdateUserAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Id = 999, Email = "test@test.com" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateUserAsync(user));
    }

    [Fact]
    public async Task UpdateUserAsync_SetsModifiedDate()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "original@test.com" });
        var service = new UserService(repository, new MockLogger());
        var updatedUser = new User { Id = 1, Email = "updated@test.com" };
        var beforeUpdate = DateTime.UtcNow;

        // Act
        await service.UpdateUserAsync(updatedUser);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(updatedUser.ModifiedDate);
        Assert.InRange(updatedUser.ModifiedDate.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesUser_WhenExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "test@test.com" });
        var service = new UserService(repository, new MockLogger());

        // Act
        await service.DeleteUserAsync(1);

        // Assert
        Assert.Empty(repository.Users);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteUserAsync(999));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsUser_WithValidCredentials()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "test@test.com" };
        var password = "password123";
        await service.CreateUserAsync(user, password);

        // Act
        var result = await service.AuthenticateAsync("test@test.com", password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WithInvalidPassword()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());
        var user = new User { Email = "test@test.com" };
        await service.CreateUserAsync(user, "correctpassword");

        // Act
        var result = await service.AuthenticateAsync("test@test.com", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsNull_WithNonExistentEmail()
    {
        // Arrange
        var repository = new MockUserRepository();
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.AuthenticateAsync("notfound@test.com", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchUsersAsync_ReturnsMatchingUsers()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe" });
        repository.Users.Add(new User { Id = 2, Email = "jane@test.com", FirstName = "Jane", LastName = "Smith" });
        repository.Users.Add(new User { Id = 3, Email = "johnny@test.com", FirstName = "Johnny", LastName = "Test" });
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.SearchUsersAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchUsersAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var repository = new MockUserRepository();
        repository.Users.Add(new User { Id = 1, Email = "test@test.com" });
        var service = new UserService(repository, new MockLogger());

        // Act
        var result = await service.SearchUsersAsync("NotFound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchUsersAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockUserRepository { ThrowException = true };
        var service = new UserService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.SearchUsersAsync("test"));
    }
}
