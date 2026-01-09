using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Repositories;

/// <summary>
/// Unit tests for TourRepository
/// </summary>
public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly TourRepository _repository;

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
    public void TourRepository_Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var repository = new TourRepository(_context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Active Tour 1", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { Id = 2, TourName = "Active Tour 2", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) },
            new Tour { Id = 3, TourName = "Inactive Tour", IsActive = false, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnToursOrderedByCreatedDateDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour 1", IsActive = true, CreatedDate = now.AddDays(-2) },
            new Tour { Id = 2, TourName = "Tour 2", IsActive = true, CreatedDate = now },
            new Tour { Id = 3, TourName = "Tour 3", IsActive = true, CreatedDate = now.AddDays(-1) }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result[0].Id);
        Assert.Equal(3, result[1].Id);
        Assert.Equal(1, result[2].Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActiveTours_ShouldReturnEmptyList()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Inactive Tour 1", IsActive = false, CreatedDate = DateTime.UtcNow },
            new Tour { Id = 2, TourName = "Inactive Tour 2", IsActive = false, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTourToDatabase()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", IsActive = true };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Tour", result.TourName);
        var dbTour = await _context.Tours.FindAsync(result.Id);
        Assert.NotNull(dbTour);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnTourWithId()
    {
        // Arrange
        var tour = new Tour { TourName = "New Tour", IsActive = true };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTourInDatabase()
    {
        // Arrange
        var tour = new Tour { TourName = "Original Tour", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.Entry(tour).State = EntityState.Detached;

        tour.TourName = "Updated Tour";

        // Act
        await _repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await _context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var tour = new Tour { TourName = "Tour to Delete", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await _context.Tours.FindAsync(tour.Id);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var tour = new Tour { TourName = "Tour to Delete", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        var beforeDelete = DateTime.UtcNow;

        // Act
        await _repository.DeleteAsync(tour.Id);
        var afterDelete = DateTime.UtcNow;

        // Assert
        var deletedTour = await _context.Tours.FindAsync(tour.Id);
        Assert.NotNull(deletedTour);
        Assert.NotNull(deletedTour.ModifiedDate);
        Assert.True(deletedTour.ModifiedDate >= beforeDelete && deletedTour.ModifiedDate <= afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Act & Assert
        await _repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ShouldReturnTrue()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Existing Tour", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentTour_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Grand Canyon Tour", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Paris Tour", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Grand Tour of Europe", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Grand");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "Arizona", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Tour 2", Place = "Paris", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Tour 3", Place = "Arizona", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Arizona");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLocations_ShouldReturnTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Locations = "Phoenix, Sedona", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Tour 2", Locations = "London, Paris", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Tour 3", Locations = "Paris, Rome", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Paris");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "Arizona", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Tour 2", Place = "Paris", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Tokyo");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Grand Tour 1", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourName = "Grand Tour 2", IsActive = false, CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Grand");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnResultsOrderedByCreatedDateDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Grand Tour 1", IsActive = true, CreatedDate = now.AddDays(-2) },
            new Tour { Id = 2, TourName = "Grand Tour 2", IsActive = true, CreatedDate = now },
            new Tour { Id = 3, TourName = "Grand Tour 3", IsActive = true, CreatedDate = now.AddDays(-1) }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.SearchAsync("Grand")).ToList();

        // Assert
        Assert.Equal(2, result[0].Id);
        Assert.Equal(3, result[1].Id);
        Assert.Equal(1, result[2].Id);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
