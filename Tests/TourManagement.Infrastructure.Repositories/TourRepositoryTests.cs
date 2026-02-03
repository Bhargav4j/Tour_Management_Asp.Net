using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;

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
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour 1", IsActive = true, CreatedBy = "System" },
            new Tour { Id = 2, TourName = "Tour 2", IsActive = true, CreatedBy = "System" },
            new Tour { Id = 3, TourName = "Tour 3", IsActive = false, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true, CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTourAndReturnIt()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", IsActive = true, CreatedBy = "System" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { TourName = "Old Name", IsActive = true, CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        tour.TourName = "Updated Name";

        // Act
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Name", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { TourName = "Test Tour", IsActive = true, CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { TourName = "Test Tour", IsActive = true, CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "France", IsActive = true, CreatedBy = "System" },
            new Tour { TourName = "London Tour", Place = "UK", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.TourName == "Paris Tour");
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ShouldReturnAllTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", IsActive = true, CreatedBy = "System" },
            new Tour { TourName = "Tour 2", IsActive = true, CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
