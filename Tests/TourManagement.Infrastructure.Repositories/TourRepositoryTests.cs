using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Tour1", Place = "Place1", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Place2", IsActive = true },
            new Tour { TourName = "Tour3", Place = "Place3", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoActiveTours_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { TourName = "Tour1", Place = "Place1", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(tour.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tour.Id, result.Id);
        Assert.Equal("Paris Tour", result.TourName);
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
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(tour.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsAndReturnsTour()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Paris", IsActive = true };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task AddAsync_SavesTourToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Paris", IsActive = true };

        // Act
        await repository.AddAsync(tour);

        // Assert
        var savedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(savedTour);
        Assert.Equal("New Tour", savedTour.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTour()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Old Name", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        context.Entry(tour).State = EntityState.Detached;

        // Act
        tour.TourName = "New Name";
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("New Name", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Tour to Delete", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(tour.Id);

        // Assert
        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_DoesNotThrow()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ReturnsTrue()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ReturnsFalse()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Paris Tour", Place = "Paris", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ReturnsFalse()
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
    public async Task SearchAsync_ByTourName_ReturnsTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", Locations = "France", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Rome", Locations = "Italy", IsActive = true },
            new Tour { TourName = "Paris Adventure", Place = "Paris", Locations = "France", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_ByPlace_ReturnsTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Tour1", Place = "Paris", Locations = "France", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Rome", Locations = "Italy", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Rome");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_ByLocations_ReturnsTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Tour1", Place = "Paris", Locations = "France, Belgium", IsActive = true },
            new Tour { TourName = "Tour2", Place = "Rome", Locations = "Italy", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Belgium");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.Add(new Tour { TourName = "Paris Tour", Place = "Paris", Locations = "France", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ExcludesInactiveTours()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TourRepository(context, _mockLogger.Object);
        context.Tours.AddRange(
            new Tour { TourName = "Paris Active", Place = "Paris", Locations = "France", IsActive = true },
            new Tour { TourName = "Paris Inactive", Place = "Paris", Locations = "France", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Paris Active", result.First().TourName);
    }
}
