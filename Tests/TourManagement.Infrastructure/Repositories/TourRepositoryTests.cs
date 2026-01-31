using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Repositories.Tests;

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
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { Id = 1, TourName = "Tour 1", Place = "Hawaii", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "Tour 2", Place = "Alps", IsActive = true });
        context.Tours.Add(new Tour { Id = 3, TourName = "Tour 3", Place = "Beach", IsActive = false });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", Place = "Hawaii", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
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
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { Id = 1, TourName = "Inactive Tour", Place = "Test", IsActive = false });
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
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Europe", Days = 10, Price = 2000.00m };

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
    public async Task AddAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Test", Place = "Test" };
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.AddAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Original", Place = "Original Place" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        tour.TourName = "Updated";
        tour.Place = "Updated Place";

        // Act
        var result = await repository.UpdateAsync(tour);

        // Assert
        Assert.Equal("Updated", result.TourName);
        Assert.Equal("Updated Place", result.Place);

        var updatedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated", updatedTour.TourName);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Test", Place = "Test" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.UpdateAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDelete()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "To Delete", Place = "Test", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteAsync(tour.Id);

        // Assert
        Assert.True(result);

        var deletedTour = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "Test", Place = "Test" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.DeleteAsync(tour.Id, cts.Token);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Test", Place = "Test", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
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
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        var tour = new Tour { TourName = "Inactive", Place = "Test", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnResults()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { TourName = "Beach Paradise", Place = "Hawaii", IsActive = true });
        context.Tours.Add(new Tour { TourName = "Mountain Adventure", Place = "Alps", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.TourName.Contains("Beach"));
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnResults()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { TourName = "Summer Vacation", Place = "Hawaii", IsActive = true });
        context.Tours.Add(new Tour { TourName = "Winter Trip", Place = "Alaska", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("Hawaii");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.Place.Contains("Hawaii"));
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ShouldReturnAll()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { TourName = "Tour 1", Place = "Place 1", IsActive = true });
        context.Tours.Add(new Tour { TourName = "Tour 2", Place = "Place 2", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);

        context.Tours.Add(new Tour { TourName = "Beach Paradise", Place = "Hawaii", IsActive = true });
        await context.SaveChangesAsync();

        // Act
        var result = await repository.SearchAsync("NotFound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.SearchAsync("test", cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
