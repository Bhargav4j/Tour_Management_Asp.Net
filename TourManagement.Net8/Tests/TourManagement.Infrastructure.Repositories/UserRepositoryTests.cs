using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _dbContextOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Id = 1, Email = "user1@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" },
            new User { Id = 2, Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", IsActive = false, Gender = "Female", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("user1@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenEmailExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task AddAsync_AddsUserSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "new@test.com", FirstName = "New", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUserSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.FirstName = "Updated";
        await repository.UpdateAsync(user);

        // Assert
        var updated = await context.Users.FindAsync(user.Id);
        Assert.Equal("Updated", updated?.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.Id);

        // Assert
        var deleted = await context.Users.FindAsync(user.Id);
        Assert.False(deleted?.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenEmailExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" },
            new User { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, Gender = "Female", PasswordHash = "hash", DateOfBirth = DateTime.Now, Street = "Street", City = "City", State = "State" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }
}
