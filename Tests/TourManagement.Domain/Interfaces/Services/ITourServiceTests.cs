using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Domain.Interfaces.Services.Tests;

public class ITourServiceTests
{
    private class TestTourService : ITourService
    {
        private readonly List<Tour> _tours = new();

        public Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Tour>>(_tours.Where(t => t.IsActive).ToList());
        }

        public Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var tour = _tours.Find(t => t.Id == id && t.IsActive);
            return Task.FromResult(tour);
        }

        public Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            tour.Id = _tours.Count + 1;
            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;
            _tours.Add(tour);
            return Task.FromResult(tour);
        }

        public Task<Tour> UpdateTourAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            var existing = _tours.Find(t => t.Id == tour.Id);
            if (existing != null)
            {
                _tours.Remove(existing);
                tour.ModifiedDate = DateTime.UtcNow;
                _tours.Add(tour);
            }
            return Task.FromResult(tour);
        }

        public Task<bool> DeleteTourAsync(int id, CancellationToken cancellationToken = default)
        {
            var tour = _tours.Find(t => t.Id == id);
            if (tour != null)
            {
                tour.IsActive = false;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var results = _tours.Where(t => t.IsActive &&
                (t.TourName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                 t.Place.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))).ToList();
            return Task.FromResult<IEnumerable<Tour>>(results);
        }
    }

    [Fact]
    public async Task GetAllToursAsync_ShouldReturnActiveTours()
    {
        // Arrange
        var service = new TestTourService();
        await service.CreateTourAsync(new Tour { TourName = "Tour 1", Place = "Hawaii" });
        await service.CreateTourAsync(new Tour { TourName = "Tour 2", Place = "Alps" });

        // Act
        var result = await service.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllToursAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetAllToursAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var service = new TestTourService();
        var createdTour = await service.CreateTourAsync(new Tour { TourName = "Test Tour", Place = "Test Place" });

        // Act
        var result = await service.GetTourByIdAsync(createdTour.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdTour.Id, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var service = new TestTourService();

        // Act
        var result = await service.GetTourByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetTourByIdAsync(1, cts.Token);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTourAsync_ShouldCreateNewTour()
    {
        // Arrange
        var service = new TestTourService();
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "Europe",
            Days = 10,
            Price = 2000.00m
        };

        // Act
        var result = await service.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Tour", result.TourName);
        Assert.True(result.IsActive);
        Assert.True(result.CreatedDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateTourAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var tour = new Tour { TourName = "Test", Place = "Test" };
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.CreateTourAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateTourAsync_ShouldUpdateExistingTour()
    {
        // Arrange
        var service = new TestTourService();
        var tour = await service.CreateTourAsync(new Tour { TourName = "Original", Place = "Original Place" });
        tour.TourName = "Updated";

        // Act
        var result = await service.UpdateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.TourName);
        Assert.NotNull(result.ModifiedDate);
    }

    [Fact]
    public async Task UpdateTourAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var tour = await service.CreateTourAsync(new Tour { TourName = "Test", Place = "Test" });
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.UpdateTourAsync(tour, cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteTourAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var service = new TestTourService();
        var tour = await service.CreateTourAsync(new Tour { TourName = "To Delete", Place = "Test" });

        // Act
        var result = await service.DeleteTourAsync(tour.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTourAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var service = new TestTourService();

        // Act
        var result = await service.DeleteTourAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTourAsync_ShouldSoftDelete()
    {
        // Arrange
        var service = new TestTourService();
        var tour = await service.CreateTourAsync(new Tour { TourName = "To Delete", Place = "Test" });

        // Act
        await service.DeleteTourAsync(tour.Id);
        var deletedTour = await service.GetTourByIdAsync(tour.Id);

        // Assert
        Assert.Null(deletedTour);
    }

    [Fact]
    public async Task DeleteTourAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.DeleteTourAsync(1, cts.Token);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchToursAsync_WithMatchingTerm_ShouldReturnResults()
    {
        // Arrange
        var service = new TestTourService();
        await service.CreateTourAsync(new Tour { TourName = "Beach Paradise", Place = "Hawaii" });
        await service.CreateTourAsync(new Tour { TourName = "Mountain Adventure", Place = "Alps" });

        // Act
        var result = await service.SearchToursAsync("Beach");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.TourName == "Beach Paradise");
    }

    [Fact]
    public async Task SearchToursAsync_WithNonMatchingTerm_ShouldReturnEmpty()
    {
        // Arrange
        var service = new TestTourService();
        await service.CreateTourAsync(new Tour { TourName = "Beach Paradise", Place = "Hawaii" });

        // Act
        var result = await service.SearchToursAsync("NotFound");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchToursAsync_WithCancellationToken_ShouldWork()
    {
        // Arrange
        var service = new TestTourService();
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.SearchToursAsync("test", cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SearchToursAsync_ShouldSearchByPlace()
    {
        // Arrange
        var service = new TestTourService();
        await service.CreateTourAsync(new Tour { TourName = "Summer Vacation", Place = "Hawaii" });
        await service.CreateTourAsync(new Tour { TourName = "Winter Trip", Place = "Alaska" });

        // Act
        var result = await service.SearchToursAsync("Hawaii");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(result, t => t.Place == "Hawaii");
    }
}
