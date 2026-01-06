using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserRepositoryTests
{
    private class MockLogger : ILogger<UserRepository>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "active@test.com", IsActive = true });
        context.Users.Add(new User { Id = 2, Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveUsers()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenUserIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByEmailAsync("inactive@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());
        var user = new User { Email = "new@test.com", FirstName = "John" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Single(context.Users);
    }

    [Fact]
    public async Task AddAsync_ReturnsUserWithGeneratedId()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());
        var user = new User { Email = "new@test.com" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var user = new User { Id = 1, Email = "original@test.com", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        context.Entry(user).State = EntityState.Detached;

        var repository = new UserRepository(context, new MockLogger());
        var updatedUser = new User { Id = 1, Email = "updated@test.com", IsActive = true };

        // Act
        await repository.UpdateAsync(updatedUser);

        // Assert
        var result = await context.Users.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal("updated@test.com", result.Email);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var user = await context.Users.FindAsync(1);
        Assert.NotNull(user);
        Assert.False(user.IsActive);
        Assert.NotNull(user.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_SetsModifiedDate()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());
        var beforeDelete = DateTime.UtcNow;

        // Act
        await repository.DeleteAsync(1);
        var afterDelete = DateTime.UtcNow;

        // Assert
        var user = await context.Users.FindAsync(1);
        Assert.NotNull(user);
        Assert.NotNull(user.ModifiedDate);
        Assert.InRange(user.ModifiedDate.Value, beforeDelete, afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenUserNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act & Assert (should not throw)
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenUserExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenUserIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenUserNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenEmailExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenEmailIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "inactive@test.com", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.EmailExistsAsync("inactive@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenEmailNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.EmailExistsAsync("notfound@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true });
        context.Users.Add(new User { Id = 2, Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true });
        context.Users.Add(new User { Id = 3, Email = "johnny@test.com", FirstName = "Johnny", LastName = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_SearchesByEmail()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true });
        context.Users.Add(new User { Id = 2, Email = "other@test.com", FirstName = "Other", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("test@");

        // Assert
        Assert.Single(result);
        Assert.Contains("test@", result.First().Email);
    }

    [Fact]
    public async Task SearchAsync_SearchesByLastName()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "user1@test.com", LastName = "Smith", IsActive = true });
        context.Users.Add(new User { Id = 2, Email = "user2@test.com", LastName = "Jones", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("Smith");

        // Assert
        Assert.Single(result);
        Assert.Equal("Smith", result.First().LastName);
    }

    [Fact]
    public async Task SearchAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "john@test.com", FirstName = "John", IsActive = true });
        context.Users.Add(new User { Id = 2, Email = "johnny@test.com", FirstName = "Johnny", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Users.Add(new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("NotFound");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SupportsCancellation()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new UserRepository(context, new MockLogger());
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
