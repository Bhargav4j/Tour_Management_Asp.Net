using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Repositories;

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
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour1", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info1", CreatedBy = "System" },
            new Tour { Id = 2, TourName = "Tour2", IsActive = false, Place = "London", Days = 3, Price = 800, Locations = "Loc2", TourInfo = "Info2", CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour1", result.First().TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTour_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Paris Tour", IsActive = true, Place = "Paris", Days = 7, Price = 1500, Locations = "Eiffel", TourInfo = "Great", CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Paris Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenTourDoesNotExist()
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
    public async Task AddAsync_ShouldAddTour_AndReturnTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Rome Tour", IsActive = true, Place = "Rome", Days = 5, Price = 1200, Locations = "Colosseum", TourInfo = "Historic", CreatedBy = "System" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Rome Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Tokyo Tour", IsActive = true, Place = "Tokyo", Days = 10, Price = 3000, Locations = "Shibuya", TourInfo = "Modern", CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated Tokyo Tour";
        await repository.UpdateAsync(tour);

        // Assert
        var updated = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated Tokyo Tour", updated!.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "London Tour", IsActive = true, Place = "London", Days = 4, Price = 1100, Locations = "BigBen", TourInfo = "Royal", CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(tour.Id);

        // Assert
        var deleted = await context.Tours.FindAsync(tour.Id);
        Assert.False(deleted!.IsActive);
        Assert.NotNull(deleted.ModifiedDate);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Berlin Tour", IsActive = true, Place = "Berlin", Days = 6, Price = 1400, Locations = "Gate", TourInfo = "History", CreatedBy = "System" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var exists = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenTourDoesNotExist()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var exists = await repository.ExistsAsync(999);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Paris Adventure", IsActive = true, Place = "Paris", Days = 5, Price = 1000, Locations = "Eiffel", TourInfo = "Amazing", CreatedBy = "System" },
            new Tour { TourName = "London Trip", IsActive = true, Place = "London", Days = 3, Price = 800, Locations = "Tower", TourInfo = "Great", CreatedBy = "System" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Adventure", result.First().TourName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenContextIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }
}
