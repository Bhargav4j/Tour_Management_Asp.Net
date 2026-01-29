using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Repositories;

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
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
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
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
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
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsNewTour()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { Name = "New Tour", Place = "Paris", IsActive = true };

        var result = await repository.AddAsync(tour);

        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTour()
    {
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Name = "Original", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        tour.Name = "Updated";
        var repository = new TourRepository(context, _mockLogger.Object);
        await repository.UpdateAsync(tour);

        var updated = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated", updated!.Name);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, Name = "To Delete", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        await repository.DeleteAsync(1);

        var deleted = await context.Tours.FindAsync(1);
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.Add(new Tour { Id = 1, Name = "Exists", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_FindsToursMatchingSearchTerm()
    {
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { Name = "Paris Adventure", Place = "France", IsActive = true },
            new Tour { Name = "Rome Tour", Place = "Italy", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        var result = await repository.SearchAsync("Paris");

        Assert.Single(result);
        Assert.Contains("Paris", result.First().Name);
    }
}
