using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly DbContextOptions<TourManagementDbContext> _dbContextOptions;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _dbContextOptions = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void TourRepository_Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void TourRepository_Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
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
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveToursExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Paris Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTourDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTourIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Price = 1000m };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);

        var savedTour = await context.Tours.FindAsync(result.Id);
        Assert.NotNull(savedTour);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTourInDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Old Name", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "New Name";
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("New Name", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Tour to Delete", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        var tourId = tour.Id;

        // Act
        await repository.DeleteAsync(tourId);

        // Assert
        var deletedTour = await context.Tours.FindAsync(tourId);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenTourDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        await repository.DeleteAsync(999);

        // Assert - No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Existing Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourIsInactive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", IsActive = true },
            new Tour { TourName = "Paris Adventure", Place = "France", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllTours_WhenSearchTermIsEmpty()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", IsActive = true },
            new Tour { TourName = "Tour 2", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmptyList_WhenNoMatchFound()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { TourName = "Paris Tour", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Tokyo");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
