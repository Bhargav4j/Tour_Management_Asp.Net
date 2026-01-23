using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
    public async Task GetAllAsync_ReturnsActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Tour 1", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { Id = 2, TourName = "Tour 2", Place = "London", Days = 3, Price = 800, Locations = "Loc2", TourInfo = "Info2", IsActive = false }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour 1", result.First().TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
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
    public async Task AddAsync_AddsTourSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "New Tour", Place = "Rome", Days = 7, Price = 1500, Locations = "Loc", TourInfo = "Info", IsActive = true };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTourSuccessfully()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated Tour";
        await repository.UpdateAsync(tour);

        // Assert
        var updated = await context.Tours.FindAsync(tour.Id);
        Assert.Equal("Updated Tour", updated?.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(tour.Id);

        // Assert
        var deleted = await context.Tours.FindAsync(tour.Id);
        Assert.False(deleted?.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTourExists()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc", TourInfo = "Info", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", Days = 3, Price = 800, Locations = "Loc2", TourInfo = "Info2", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Tour", result.First().TourName);
    }

    [Fact]
    public async Task GetByPlaceAsync_ReturnsToursForPlace()
    {
        // Arrange
        using var context = new TourManagementDbContext(_dbContextOptions);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour 1", Place = "Paris", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "Paris Tour 2", Place = "Paris", Days = 7, Price = 1200, Locations = "Loc2", TourInfo = "Info2", IsActive = true },
            new Tour { TourName = "London Tour", Place = "London", Days = 3, Price = 800, Locations = "Loc3", TourInfo = "Info3", IsActive = true }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByPlaceAsync("Paris");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, tour => Assert.Equal("Paris", tour.Place));
    }
}
