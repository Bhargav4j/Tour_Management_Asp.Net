using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveUsers()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.AddRange(
            new User { Email = "active@test.com", FirstName = "Active", IsActive = true },
            new User { Email = "inactive@test.com", FirstName = "Inactive", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.Add(new User { Email = "test@example.com", FirstName = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.GetByEmailAsync("test@example.com");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var result = await repository.GetByEmailAsync("notfound@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsNewUser()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "newuser@test.com", FirstName = "New", IsActive = true };

        var result = await repository.AddAsync(user);

        Assert.NotNull(result);
        Assert.Equal("newuser@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingUser()
    {
        using var context = new TourManagementDbContext(_options);
        var user = new User { Email = "user@test.com", FirstName = "Original", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        user.FirstName = "Updated";
        var repository = new UserRepository(context, _mockLogger.Object);
        await repository.UpdateAsync(user);

        var updated = await context.Users.FindAsync("user@test.com");
        Assert.Equal("Updated", updated!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        using var context = new TourManagementDbContext(_options);
        var user = new User { Email = "delete@test.com", FirstName = "Delete", IsActive = true };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        await repository.DeleteAsync("delete@test.com");

        var deleted = await context.Users.FindAsync("delete@test.com");
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingEmail_ReturnsTrue()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.Add(new User { Email = "exists@test.com", FirstName = "Exists", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.ExistsAsync("exists@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingEmail_ReturnsFalse()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var result = await repository.ExistsAsync("notfound@test.com");

        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidEmail_ReturnsUser()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.Add(new User { Email = "auth@test.com", FirstName = "Auth", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.AuthenticateAsync("auth@test.com", "password");

        Assert.NotNull(result);
        Assert.Equal("auth@test.com", result.Email);
    }
}
