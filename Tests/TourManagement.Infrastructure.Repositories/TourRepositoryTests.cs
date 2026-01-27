using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;

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
    public void TourRepository_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act
        var repository = new TourRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void TourRepository_Constructor_WithNullContext_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void TourRepository_Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour1 = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour2 = new Tour { TourName = "Rome Tour", Place = "Rome", Days = 6, Price = 1200m, IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) };
        var tour3 = new Tour { TourName = "Berlin Tour", Place = "Berlin", Days = 4, Price = 800m, IsActive = false, CreatedDate = DateTime.UtcNow };

        context.Tours.AddRange(tour1, tour2, tour3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.TourName == "Paris Tour");
        Assert.Contains(result, t => t.TourName == "Rome Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 3, Price = 500m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(tour.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tour.Id, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
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
    public async Task AddAsync_AddsTourToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 7,
            Price = 1500m,
            Locations = "Location1, Location2",
            TourInfo = "Great tour",
            IsActive = true,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "System"
        };

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
    public async Task UpdateAsync_UpdatesExistingTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Original Tour", Place = "Original Place", Days = 5, Price = 1000m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        context.Entry(tour).State = EntityState.Detached;

        // Act
        tour.TourName = "Updated Tour";
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Delete Tour", Place = "Delete Place", Days = 3, Price = 600m, IsActive = true, CreatedDate = DateTime.UtcNow };
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
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingTour_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Exists Tour", Place = "Exists Place", Days = 5, Price = 900m, IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingTour_ReturnsFalse()
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
    public async Task SearchAsync_WithMatchingTerm_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour1 = new Tour { TourName = "Paris Adventure", Place = "Paris", Days = 5, Price = 1000m, Locations = "Eiffel Tower, Louvre", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour2 = new Tour { TourName = "Rome Expedition", Place = "Rome", Days = 6, Price = 1200m, Locations = "Colosseum", IsActive = true, CreatedDate = DateTime.UtcNow };
        var tour3 = new Tour { TourName = "Berlin Trip", Place = "Berlin", Days = 4, Price = 800m, Locations = "Brandenburg Gate", IsActive = true, CreatedDate = DateTime.UtcNow };

        context.Tours.AddRange(tour1, tour2, tour3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.TourName == "Paris Adventure");
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 3, Price = 500m, Locations = "Test Location", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NoMatch");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_SearchesByTourName_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Unique Tour Name", Place = "Place", Days = 5, Price = 1000m, Locations = "Locations", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Unique");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Unique Tour Name", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_SearchesByPlace_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Tour", Place = "UniquePlace", Days = 5, Price = 1000m, Locations = "Locations", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("UniquePlace");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("UniquePlace", result.First().Place);
    }

    [Fact]
    public async Task SearchAsync_SearchesByLocations_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Tour", Place = "Place", Days = 5, Price = 1000m, Locations = "UniqueLocation", IsActive = true, CreatedDate = DateTime.UtcNow };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("UniqueLocation");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains("UniqueLocation", result.First().Locations);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbOptions);
        var repository = new TourRepository(context, _mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
