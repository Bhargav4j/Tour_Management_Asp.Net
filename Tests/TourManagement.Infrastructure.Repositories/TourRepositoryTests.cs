using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveTours()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { Id = 1, Name = "Active Tour", IsActive = true },
            new Tour { Id = 2, Name = "Inactive Tour", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Active Tour", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenExists()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.Add(new Tour { Id = 1, Name = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.Name);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { Name = "New Tour", Place = "Test" };

        var result = await repository.AddAsync(tour);

        Assert.NotEqual(0, result.Id);
        Assert.Equal(1, await context.Tours.CountAsync());
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTourExists()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.Add(new Tour { Id = 1, Name = "Test", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourNotExists()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { Name = "Beach Tour", Place = "Hawaii", IsActive = true },
            new Tour { Name = "Mountain Tour", Place = "Alps", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.SearchAsync("Beach");

        Assert.Single(result);
        Assert.Equal("Beach Tour", result.First().Name);
    }
}
