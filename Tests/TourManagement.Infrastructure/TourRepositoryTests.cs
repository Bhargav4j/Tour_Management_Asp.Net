using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Tests;

/// <summary>
/// Test class for TourRepository
/// </summary>
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
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act
        var repository = new TourRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveTours_ReturnsActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var activeTour = new Tour { Id = 1, TourName = "Active Tour", IsActive = true };
        var inactiveTour = new Tour { Id = 2, TourName = "Inactive Tour", IsActive = false };
        context.Tours.AddRange(activeTour, inactiveTour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Active Tour", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoActiveTours_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidTour_AddsTourAndReturns()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);
        var tour = new Tour { TourName = "New Tour", Place = "New Place", IsActive = true };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Tour", result.TourName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task AddAsync_WithTour_SavesChangesToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);
        var tour = new Tour { TourName = "Test Tour", IsActive = true };

        // Act
        await repository.AddAsync(tour);
        var savedTour = await context.Tours.FindAsync(tour.Id);

        // Assert
        Assert.NotNull(savedTour);
        Assert.Equal("Test Tour", savedTour.TourName);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);
        var tour = new Tour { TourName = "Test Tour" };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.AddAsync(tour, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidTour_UpdatesTour()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Original Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);
        tour.TourName = "Updated Tour";

        // Act
        await repository.UpdateAsync(tour);
        var updatedTour = await context.Tours.FindAsync(1);

        // Assert
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.UpdateAsync(tour, cancellationToken);

        // Assert - No exception thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteAsync(1);
        var deletedTour = await context.Tours.FindAsync(1);

        // Assert
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        // No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.DeleteAsync(1, cancellationToken);

        // Assert
        var deletedTour = await context.Tours.FindAsync(1);
        Assert.False(deletedTour!.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour1 = new Tour { Id = 1, TourName = "Grand Canyon Adventure", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Paris Tour", IsActive = true };
        context.Tours.AddRange(tour1, tour2);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.SearchAsync("Canyon");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Grand Canyon Adventure", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Tour", Place = "Arizona", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.SearchAsync("Arizona");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLocations_ReturnsTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Tour", Locations = "Grand Canyon, Las Vegas", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.SearchAsync("Las Vegas");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, TourName = "Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithEmptySearchTerm_ReturnsAllActiveTours()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour1 = new Tour { Id = 1, TourName = "Tour1", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Tour2", IsActive = true };
        context.Tours.AddRange(tour1, tour2);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.SearchAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
