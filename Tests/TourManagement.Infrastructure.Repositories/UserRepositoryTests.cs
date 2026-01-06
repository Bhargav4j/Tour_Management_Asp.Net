using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveUsers()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.AddRange(
            new User { Id = 1, Email = "active@test.com", IsActive = true },
            new User { Id = 2, Email = "inactive@test.com", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("active@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.Add(new User { Id = 1, Email = "test@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.GetByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);
        var user = new User { Email = "new@test.com", FirstName = "Test" };

        var result = await repository.AddAsync(user);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenExists()
    {
        using var context = new TourManagementDbContext(_options);
        context.Users.Add(new User { Email = "existing@test.com", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mockLogger.Object);
        var result = await repository.EmailExistsAsync("existing@test.com");

        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenNotExists()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var result = await repository.EmailExistsAsync("notfound@test.com");

        Assert.False(result);
    }
}
