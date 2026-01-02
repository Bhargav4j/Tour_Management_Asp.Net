using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Repositories;

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
    public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Id = 1, Email = "user1@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St1", City = "City1", State = "ST1", CreatedBy = "System" },
            new User { Id = 2, Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", IsActive = false, Gender = "Female", PasswordHash = "hash", Street = "St2", City = "City2", State = "ST2", CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("user1@test.com", result.First().Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Id = 1, Email = "test@example.com", FirstName = "Bob", LastName = "Johnson", IsActive = true, Gender = "Male", PasswordHash = "hash123", Street = "Main St", City = "Boston", State = "MA", CreatedBy = "System" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "unique@test.com", FirstName = "Alice", LastName = "Williams", IsActive = true, Gender = "Female", PasswordHash = "hash456", Street = "Oak Ave", City = "Seattle", State = "WA", CreatedBy = "System" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("unique@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.FirstName);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser_AndReturnUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "new@user.com", FirstName = "Tom", LastName = "Brown", IsActive = true, Gender = "Male", PasswordHash = "hash789", Street = "Pine Rd", City = "Denver", State = "CO", CreatedBy = "System" };

        // Act
        var result = await repository.AddAsync(user);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("new@user.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "update@test.com", FirstName = "Old", LastName = "Name", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "Street", City = "City", State = "ST", CreatedBy = "System" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        user.FirstName = "New";
        await repository.UpdateAsync(user);

        // Assert
        var updated = await context.Users.FindAsync(user.Id);
        Assert.Equal("New", updated!.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteUser()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "delete@test.com", FirstName = "Delete", LastName = "Me", IsActive = true, Gender = "Female", PasswordHash = "hash", Street = "Street", City = "City", State = "ST", CreatedBy = "System" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(user.Id);

        // Assert
        var deleted = await context.Users.FindAsync(user.Id);
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        var user = new User { Email = "exists@test.com", FirstName = "Exists", LastName = "User", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "Street", City = "City", State = "ST", CreatedBy = "System" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(user.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingUsers()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new UserRepository(context, _mockLogger.Object);

        context.Users.AddRange(
            new User { Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true, Gender = "Male", PasswordHash = "hash", Street = "St1", City = "City1", State = "ST1", CreatedBy = "System" },
            new User { Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true, Gender = "Female", PasswordHash = "hash", Street = "St2", City = "City2", State = "ST2", CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("John");

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(context, null!));
    }
}
