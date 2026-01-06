using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class TourServiceTests
{
    private class MockTourRepository : ITourRepository
    {
        public List<Tour> Tours { get; set; } = new List<Tour>();
        public bool ThrowException { get; set; }

        public Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Tours.AsEnumerable());
        }

        public Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Tours.FirstOrDefault(t => t.Id == id));
        }

        public Task<Tour> AddAsync(Tour entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            entity.Id = Tours.Count + 1;
            Tours.Add(entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(Tour entity, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var existing = Tours.FirstOrDefault(t => t.Id == entity.Id);
            if (existing != null)
            {
                var index = Tours.IndexOf(existing);
                Tours[index] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var tour = Tours.FirstOrDefault(t => t.Id == id);
            if (tour != null)
            {
                Tours.Remove(tour);
            }
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Tours.Any(t => t.Id == id));
        }

        public Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            var results = Tours.Where(t => t.TourName.Contains(searchTerm) || t.Place.Contains(searchTerm));
            return Task.FromResult(results.AsEnumerable());
        }
    }

    private class MockLogger : ILogger<TourService>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        // Arrange
        var logger = new MockLogger();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(null!, logger));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var repository = new MockTourRepository();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourService(repository, null!));
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsAllTours()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Tour 1" });
        repository.Tours.Add(new Tour { Id = 2, TourName = "Tour 2" });
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsEmptyList_WhenNoTours()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.GetAllToursAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllToursAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockTourRepository { ThrowException = true };
        var service = new TourService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetAllToursAsync());
    }

    [Fact]
    public async Task GetTourByIdAsync_ReturnsTour_WhenExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Test Tour" });
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.GetTourByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetTourByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.GetTourByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTourByIdAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockTourRepository { ThrowException = true };
        var service = new TourService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetTourByIdAsync(1));
    }

    [Fact]
    public async Task CreateTourAsync_AddsTour()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());
        var tour = new Tour { TourName = "New Tour", Place = "Paris" };

        // Act
        var result = await service.CreateTourAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.True(result.IsActive);
        Assert.Single(repository.Tours);
    }

    [Fact]
    public async Task CreateTourAsync_SetsCreatedDate()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());
        var tour = new Tour { TourName = "New Tour" };
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await service.CreateTourAsync(tour);
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(result.CreatedDate, beforeCreation, afterCreation);
    }

    [Fact]
    public async Task CreateTourAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockTourRepository { ThrowException = true };
        var service = new TourService(repository, new MockLogger());
        var tour = new Tour { TourName = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.CreateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_UpdatesTour_WhenExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Original" });
        var service = new TourService(repository, new MockLogger());
        var updatedTour = new Tour { Id = 1, TourName = "Updated" };

        // Act
        await service.UpdateTourAsync(updatedTour);

        // Assert
        var tour = repository.Tours.First();
        Assert.Equal("Updated", tour.TourName);
        Assert.NotNull(tour.ModifiedDate);
    }

    [Fact]
    public async Task UpdateTourAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());
        var tour = new Tour { Id = 999, TourName = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateTourAsync(tour));
    }

    [Fact]
    public async Task UpdateTourAsync_SetsModifiedDate()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Original" });
        var service = new TourService(repository, new MockLogger());
        var updatedTour = new Tour { Id = 1, TourName = "Updated" };
        var beforeUpdate = DateTime.UtcNow;

        // Act
        await service.UpdateTourAsync(updatedTour);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(updatedTour.ModifiedDate);
        Assert.InRange(updatedTour.ModifiedDate.Value, beforeUpdate, afterUpdate);
    }

    [Fact]
    public async Task DeleteTourAsync_DeletesTour_WhenExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Test" });
        var service = new TourService(repository, new MockLogger());

        // Act
        await service.DeleteTourAsync(1);

        // Assert
        Assert.Empty(repository.Tours);
    }

    [Fact]
    public async Task DeleteTourAsync_ThrowsInvalidOperationException_WhenNotExists()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteTourAsync(999));
    }

    [Fact]
    public async Task DeleteTourAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockTourRepository { ThrowException = true };
        var service = new TourService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.DeleteTourAsync(1));
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsMatchingTours()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Paris Tour", Place = "France" });
        repository.Tours.Add(new Tour { Id = 2, TourName = "London Tour", Place = "England" });
        repository.Tours.Add(new Tour { Id = 3, TourName = "Paris Adventure", Place = "France" });
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.SearchToursAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var repository = new MockTourRepository();
        repository.Tours.Add(new Tour { Id = 1, TourName = "Paris Tour" });
        var service = new TourService(repository, new MockLogger());

        // Act
        var result = await service.SearchToursAsync("Tokyo");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchToursAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var repository = new MockTourRepository { ThrowException = true };
        var service = new TourService(repository, new MockLogger());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.SearchToursAsync("test"));
    }

    [Fact]
    public async Task GetAllToursAsync_SupportsCancellation()
    {
        // Arrange
        var repository = new MockTourRepository();
        var service = new TourService(repository, new MockLogger());
        var cts = new CancellationTokenSource();

        // Act
        var result = await service.GetAllToursAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }
}
