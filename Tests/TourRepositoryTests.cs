using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbOptions;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _dbOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private TourManagementDbContext CreateContext()
    {
        return new TourManagementDbContext(_dbOptions);
    }

    [Fact]
    public void Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour1", IsActive = true },
            new Tour { Id = 2, TourName = "Tour2", IsActive = true },
            new Tour { Id = 3, TourName = "Tour3", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.DoesNotContain(result, t => t.Id == 3);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour1", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tour1", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour1", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Paris", Days = 7, Price = 1999m };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTourInDatabase()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Original", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);
        tour.TourName = "Updated";

        // Act
        var result = await repository.UpdateAsync(tour);

        // Assert
        Assert.Equal("Updated", result.TourName);
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated", updatedTour?.TourName);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateContext();
        var tour = new Tour { TourName = "Tour1", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(tour.Id);

        // Assert
        Assert.True(result);
        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.False(deletedTour?.IsActive);
        Assert.NotNull(deletedTour?.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ReturnsTrue()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour1", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour1", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ReturnsMatchingTours()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.AddRange(
            new Tour { TourName = "Paris Adventure", Place = "Paris", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Rome", IsActive = true },
            new Tour { TourName = "Paris Express", Place = "Paris", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Contains("Paris", t.TourName + t.Place));
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ReturnsMatchingTours()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.AddRange(
            new Tour { TourName = "Tour1", Place = "Rome", Locations = "Rome, Vatican", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Paris", Locations = "Paris, Louvre", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Rome");

        // Assert
        Assert.Single(result);
        Assert.Equal("Rome", result.First().Place);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllActiveTours()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.AddRange(
            new Tour { TourName = "Tour1", IsActive = true },
            new Tour { TourName = "Tour2", IsActive = true },
            new Tour { TourName = "Tour3", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        context.Tours.Add(new Tour { TourName = "Tour1", Place = "Paris", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Tokyo");

        // Assert
        Assert.Empty(result);
    }
}
