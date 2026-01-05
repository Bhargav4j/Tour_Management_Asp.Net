using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _loggerMock;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<TourRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);

        // Act
        var repository = new TourRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnActiveTours()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour1 = new Tour { TourId = 1, TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100m, IsActive = true };
        var tour2 = new Tour { TourId = 2, TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200m, IsActive = true };
        var tour3 = new Tour { TourId = 3, TourName = "Tour 3", Place = "Place 3", Days = 3, Price = 150m, IsActive = false };

        await context.Tours.AddRangeAsync(tour1, tour2, tour3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithNoTours_ShouldReturnEmptyList()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourId = 1, TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = true };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

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
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourId = 1, TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = false };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTour()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "New Tour", Place = "New Place", Days = 5, Price = 100m, IsActive = true };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.TourId > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTour()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Original Tour", Place = "Original Place", Days = 5, Price = 100m, IsActive = true };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        tour.TourName = "Updated Tour";
        await repository.UpdateAsync(tour);

        // Assert
        var updatedTour = await context.Tours.FindAsync(tour.TourId);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkTourAsInactive()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = true };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(tour.TourId);

        // Assert
        var deletedTour = await context.Tours.FindAsync(tour.TourId);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = true };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.TourId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = false };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.TourId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnTours()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour1 = new Tour { TourName = "Beach Tour", Place = "Miami", Days = 5, Price = 100m, IsActive = true };
        var tour2 = new Tour { TourName = "Mountain Tour", Place = "Alps", Days = 7, Price = 200m, IsActive = true };

        await context.Tours.AddRangeAsync(tour1, tour2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Beach");

        // Assert
        Assert.Single(result);
        Assert.Equal("Beach Tour", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnTours()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour1 = new Tour { TourName = "Tour 1", Place = "Paris", Days = 5, Price = 100m, IsActive = true };
        var tour2 = new Tour { TourName = "Tour 2", Place = "London", Days = 7, Price = 200m, IsActive = true };

        await context.Tours.AddRangeAsync(tour1, tour2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris", result.First().Place);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100m, IsActive = true };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        // Arrange
        var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        var tour = new Tour { TourName = "Beach Tour", Place = "Miami", Days = 5, Price = 100m, IsActive = false };
        await context.Tours.AddAsync(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Beach");

        // Assert
        Assert.Empty(result);
    }
}
