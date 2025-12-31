using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

/// <summary>
/// Unit tests for TourRepository
/// </summary>
public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour 1", IsActive = true },
            new Tour { Id = 2, TourName = "Tour 2", IsActive = true },
            new Tour { Id = 3, TourName = "Tour 3", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ReturnsNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsTourSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "Paris",
            Price = 999.99m,
            IsActive = true
        };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTourSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Old Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated Tour";
        await repository.UpdateAsync(tour);

        // Assert
        var updated = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated Tour", updated!.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.False(deletedTour!.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Rome", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Tour", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByPlaceAsync_ReturnsToursForPlace()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "Hawaii", IsActive = true },
            new Tour { TourName = "Tour 2", Place = "Hawaii", IsActive = true },
            new Tour { TourName = "Tour 3", Place = "Alaska", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByPlaceAsync("Hawaii");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal("Hawaii", t.Place));
    }

    [Fact]
    public async Task GetByPlaceAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByPlaceAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }
}
