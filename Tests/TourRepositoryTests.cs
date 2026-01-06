using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Repositories.Tests;

public class TourRepositoryTests
{
    private class MockLogger : ILogger<TourRepository>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    private TourManagementDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new TourManagementDbContext(options);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenContextIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var context = CreateInMemoryContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveTours()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Active Tour", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "Inactive Tour", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoActiveTours()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTour_WhenExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTourIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());
        var tour = new Tour { TourName = "New Tour", Place = "Paris" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Single(context.Tours);
    }

    [Fact]
    public async Task AddAsync_ReturnsTourWithGeneratedId()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());
        var tour = new Tour { TourName = "New Tour" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTour()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var tour = new Tour { Id = 1, TourName = "Original", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();
        context.Entry(tour).State = EntityState.Detached;

        var repository = new TourRepository(context, new MockLogger());
        var updatedTour = new Tour { Id = 1, TourName = "Updated", IsActive = true };

        // Act
        await repository.UpdateAsync(updatedTour);

        // Assert
        var result = await context.Tours.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Updated", result.TourName);
    }

    [Fact]
    public async Task DeleteAsync_SetsIsActiveToFalse()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        await repository.DeleteAsync(1);

        // Assert
        var tour = await context.Tours.FindAsync(1);
        Assert.NotNull(tour);
        Assert.False(tour.IsActive);
        Assert.NotNull(tour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_SetsModifiedDate()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());
        var beforeDelete = DateTime.UtcNow;

        // Act
        await repository.DeleteAsync(1);
        var afterDelete = DateTime.UtcNow;

        // Assert
        var tour = await context.Tours.FindAsync(1);
        Assert.NotNull(tour);
        Assert.NotNull(tour.ModifiedDate);
        Assert.InRange(tour.ModifiedDate.Value, beforeDelete, afterDelete);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenTourNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());

        // Act & Assert (should not throw)
        await repository.DeleteAsync(999);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTourExistsAndActive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Test Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourIsInactive()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTourNotExists()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingTours()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Paris Tour", Place = "France", Locations = "Eiffel Tower", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "London Tour", Place = "England", Locations = "Big Ben", IsActive = true });
        context.Tours.Add(new Tour { Id = 3, TourName = "Paris Adventure", Place = "France", Locations = "Louvre", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_SearchesByPlace()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour 1", Place = "France", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "Tour 2", Place = "England", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("France");

        // Assert
        Assert.Single(result);
        Assert.Equal("France", result.First().Place);
    }

    [Fact]
    public async Task SearchAsync_SearchesByLocations()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Tour 1", Locations = "Eiffel Tower, Louvre", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "Tour 2", Locations = "Big Ben", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("Eiffel");

        // Assert
        Assert.Single(result);
        Assert.Contains("Eiffel", result.First().Locations);
    }

    [Fact]
    public async Task SearchAsync_ReturnsOnlyActiveTours()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Paris Tour", IsActive = true });
        context.Tours.Add(new Tour { Id = 2, TourName = "Paris Adventure", IsActive = false });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var context = CreateInMemoryContext();
        context.Tours.Add(new Tour { Id = 1, TourName = "Paris Tour", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, new MockLogger());

        // Act
        var result = await repository.SearchAsync("Tokyo");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SupportsCancellation()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repository = new TourRepository(context, new MockLogger());
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
