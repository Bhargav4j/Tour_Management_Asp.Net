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

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new TourRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
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
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Tour 1", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 2, TourName = "Tour 2", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) },
            new Tour { TourId = 3, TourName = "Tour 3", IsActive = false, CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithNoTours_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByCreatedDateDescending()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var date1 = DateTime.UtcNow.AddDays(-2);
        var date2 = DateTime.UtcNow.AddDays(-1);
        var date3 = DateTime.UtcNow;
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Tour 1", IsActive = true, CreatedDate = date1 },
            new Tour { TourId = 2, TourName = "Tour 2", IsActive = true, CreatedDate = date3 },
            new Tour { TourId = 3, TourName = "Tour 3", IsActive = true, CreatedDate = date2 }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result[0].TourId);
        Assert.Equal(3, result[1].TourId);
        Assert.Equal(1, result[2].TourId);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourId = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TourId);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.Add(new Tour { TourId = 1, TourName = "Inactive Tour", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidTour_ShouldAddAndReturnTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Paris" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.TourId);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour" };

        // Act
        await repository.AddAsync(tour);

        // Assert
        var savedTour = await context.Tours.FirstOrDefaultAsync(t => t.TourName == "New Tour");
        Assert.NotNull(savedTour);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingTour_ShouldUpdateTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourId = 1, TourName = "Original Name", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        context.Entry(tour).State = EntityState.Detached;

        var repository = new TourRepository(context, _mockLogger.Object);
        tour.TourName = "Updated Name";

        // Act
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(1);
        Assert.Equal("Updated Name", updatedTour!.TourName);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingTour_ShouldSetIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourId = 1, TourName = "Tour to Delete", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deletedTour = await context.Tours.FindAsync(1);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var tour = new Tour { TourId = 1, TourName = "Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var deletedTour = await context.Tours.FindAsync(1);
        Assert.NotNull(deletedTour!.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingTour_ShouldNotThrowException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        // No exception should be thrown
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.Add(new Tour { TourId = 1, TourName = "Tour", IsActive = true });
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingTour_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.Add(new Tour { TourId = 1, TourName = "Inactive Tour", IsActive = false });
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithTourNameMatch_ShouldReturnMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Paris Adventure", Place = "France", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 2, TourName = "Rome Tour", Place = "Italy", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 3, TourName = "Paris Night", Place = "France", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithPlaceMatch_ShouldReturnMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Tour 1", Place = "France", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 2, TourName = "Tour 2", Place = "Italy", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("France");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithLocationsMatch_ShouldReturnMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Tour 1", Locations = "Eiffel Tower", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 2, TourName = "Tour 2", Locations = "Colosseum", IsActive = true, CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Eiffel");

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatch_ShouldReturnEmptyCollection()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.Add(new Tour { TourId = 1, TourName = "Tour", Place = "Place", IsActive = true, CreatedDate = DateTime.UtcNow });
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExisting");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        context.Tours.AddRange(
            new Tour { TourId = 1, TourName = "Paris Tour", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Tour { TourId = 2, TourName = "Paris Adventure", IsActive = false, CreatedDate = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.All(result, t => Assert.True(t.IsActive));
    }
}
