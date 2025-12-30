using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.IntegrationTests.Repositories;

public class UserRepositoryTests
{
    private readonly TourManagementDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        var mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveUsers()
    {
        // Arrange
        _context.Users.AddRange(
            new User { Email = "test1@test.com", FirstName = "Test1", IsActive = true, PasswordHash = "hash" },
            new User { Email = "test2@test.com", FirstName = "Test2", IsActive = false, PasswordHash = "hash" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be("test1@test.com");
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        var user = new User
        {
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var savedUser = await _context.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("newuser@test.com");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
