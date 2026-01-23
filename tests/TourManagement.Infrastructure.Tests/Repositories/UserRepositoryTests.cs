using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _userRepository = new UserRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserRepository(_context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "user1@test.com", FirstName = "User1", IsActive = true },
            new User { Id = 2, Email = "user2@test.com", FirstName = "User2", IsActive = true },
            new User { Id = 3, Email = "user3@test.com", FirstName = "User3", IsActive = false }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.True(u.IsActive));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingActiveUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Act
        var result = await _userRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("notfound@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = new User
        {
            Email = "new@test.com",
            FirstName = "New",
            LastName = "User",
            IsActive = true
        };

        // Act
        var result = await _userRepository.AddAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedUser = await _context.Users.FindAsync(result.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("new@test.com", savedUser.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Old", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached;

        user.FirstName = "Updated";

        // Act
        await _userRepository.UpdateAsync(user);

        // Assert
        var updatedUser = await _context.Users.FindAsync(1);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetUserAsInactive()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _userRepository.DeleteAsync(1);

        // Assert
        var deletedUser = await _context.Users.FindAsync(1);
        Assert.NotNull(deletedUser);
        Assert.False(deletedUser.IsActive);
        Assert.NotNull(deletedUser.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingUser_ShouldNotThrow()
    {
        // Act & Assert
        await _userRepository.DeleteAsync(999);
        // Should not throw exception
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveUser_ShouldReturnTrue()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Act
        var result = await _userRepository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = false };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.EmailExistsAsync("test@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Act
        var result = await _userRepository.EmailExistsAsync("notfound@test.com");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingFirstName_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Id = 2, Email = "johnny@test.com", FirstName = "Johnny", LastName = "Smith", IsActive = true },
            new User { Id = 3, Email = "jane@test.com", FirstName = "Jane", LastName = "Doe", IsActive = true }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLastName_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Id = 2, Email = "jane@test.com", FirstName = "Jane", LastName = "Doe", IsActive = true }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.SearchAsync("Doe");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingEmail_ShouldReturnUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Id = 2, Email = "jane@test.com", FirstName = "Jane", LastName = "Smith", IsActive = true }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.SearchAsync("test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyCollection()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", IsActive = true };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.SearchAsync("xyz");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldExcludeInactiveUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "john@test.com", FirstName = "John", LastName = "Doe", IsActive = true },
            new User { Id = 2, Email = "johnny@test.com", FirstName = "Johnny", LastName = "Smith", IsActive = false }
        };
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.SearchAsync("John");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, u => Assert.True(u.IsActive));
    }
}
