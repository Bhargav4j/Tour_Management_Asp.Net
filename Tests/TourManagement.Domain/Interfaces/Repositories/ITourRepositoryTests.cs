using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Domain.Interfaces.Repositories.Tests;

public class ITourRepositoryTests
{
    private class TestTourRepository : ITourRepository
    {
        private readonly List<Tour> _tours = new();

        public Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Tour>>(_tours);
        }

        public Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var tour = _tours.Find(t => t.Id == id);
            return Task.FromResult(tour);
        }

        public Task<Tour> AddAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            tour.Id = _tours.Count + 1;
            _tours.Add(tour);
            return Task.FromResult(tour);
        }

        public Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            var existing = _tours.Find(t => t.Id == tour.Id);
            if (existing != null)
            {
                _tours.Remove(existing);
                _tours.Add(tour);
            }
            return Task.FromResult(tour);
        }

        public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var tour = _tours.Find(t => t.Id == id);
            if (tour != null)
            {
                _tours.Remove(tour);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_tours.Exists(t => t.Id == id));
        }

        public Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var results = _tours.FindAll(t =>
                t.TourName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                t.Place.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult<IEnumerable<Tour>>(results);
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var repository = new TestTourRepository();
        await repository.AddAsync(new Tour { TourName = "Tour 1" });
        await repository.AddAsync(new Tour { TourName = "Tour 2" });

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<Tour>)result).Count);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
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
        var repository = new TestTourRepository();
        var addedTour = await repository.AddAsync(new Tour { TourName = "Test Tour" });

        // Act
        var result = await repository.GetByIdAsync(addedTour.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(addedTour.Id, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var repository = new TestTourRepository();

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.GetByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTour()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = new Tour { TourName = "New Tour", Place = "Hawaii" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = new Tour { TourName = "New Tour" };
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.AddAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingTour()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = await repository.AddAsync(new Tour { TourName = "Original" });
        tour.TourName = "Updated";

        // Act
        var result = await repository.UpdateAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.TourName);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = await repository.AddAsync(new Tour { TourName = "Test" });
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.UpdateAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = await repository.AddAsync(new Tour { TourName = "To Delete" });

        // Act
        var result = await repository.DeleteAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestTourRepository();

        // Act
        var result = await repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.DeleteAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var repository = new TestTourRepository();
        var tour = await repository.AddAsync(new Tour { TourName = "Test" });

        // Act
        var result = await repository.ExistsAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var repository = new TestTourRepository();

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var repository = new TestTourRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.ExistsAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var repository = new TestTourRepository();
        await repository.AddAsync(new Tour { TourName = "Beach Paradise", Place = "Hawaii" });
        await repository.AddAsync(new Tour { TourName = "Mountain Adventure", Place = "Alps" });

        // Act
        var result = await repository.SearchAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        var repository = new TestTourRepository();
        await repository.AddAsync(new Tour { TourName = "Beach Paradise" });

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
        var repository = new TestTourRepository();
        var cts = new CancellationTokenSource();

        // Act
        var result = await repository.SearchAsync("test", cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
