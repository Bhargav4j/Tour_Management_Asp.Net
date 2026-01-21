using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.IntegrationTests.Repositories;

public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var savedTour = await _context.Tours.FindAsync(result.Id);
        savedTour.Should().NotBeNull();
        savedTour!.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveTours()
    {
        // Arrange
        await _context.Tours.AddRangeAsync(
            new Tour { TourName = "Active Tour", Place = "Place 1", Days = 5, Price = 1000, IsActive = true },
            new Tour { TourName = "Inactive Tour", Place = "Place 2", Days = 3, Price = 500, IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Active Tour");
    }

    [Fact]
    public async Task SearchAsync_FindsMatchingTours()
    {
        // Arrange
        await _context.Tours.AddRangeAsync(
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1000, IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", Days = 3, Price = 800, IsActive = true }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Paris");

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Contain("Paris");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
