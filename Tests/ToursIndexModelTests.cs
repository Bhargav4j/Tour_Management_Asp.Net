using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Tours;

namespace TourManagement.Web.Pages.Tours.Tests;

public class ToursIndexModelTests
{
    private class MockTourService : ITourService
    {
        public List<Tour> Tours { get; set; } = new List<Tour>();
        public bool ThrowException { get; set; }

        public Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(Tours.AsEnumerable());
        }

        public Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Tours.FirstOrDefault(t => t.Id == id));
        }

        public Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            tour.Id = Tours.Count + 1;
            Tours.Add(tour);
            return Task.FromResult(tour);
        }

        public Task UpdateTourAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteTourAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Tours.Where(t => t.TourName.Contains(searchTerm)).AsEnumerable());
        }
    }

    private class MockLogger : ILogger<IndexModel>
    {
        public int LogErrorCallCount { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Error)
            {
                LogErrorCallCount++;
            }
        }
    }

    [Fact]
    public void Constructor_InitializesModel()
    {
        // Arrange
        var tourService = new MockTourService();
        var logger = new MockLogger();

        // Act
        var model = new IndexModel(tourService, logger);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public async Task OnGetAsync_LoadsTours()
    {
        // Arrange
        var tourService = new MockTourService();
        tourService.Tours.Add(new Tour { Id = 1, TourName = "Tour 1" });
        tourService.Tours.Add(new Tour { Id = 2, TourName = "Tour 2" });
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.NotNull(model.Tours);
        Assert.Equal(2, model.Tours.Count());
    }

    [Fact]
    public async Task OnGetAsync_InitializesTours_WhenServiceReturnsEmpty()
    {
        // Arrange
        var tourService = new MockTourService();
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.NotNull(model.Tours);
        Assert.Empty(model.Tours);
    }

    [Fact]
    public async Task OnGetAsync_SetsMessage_WhenExceptionOccurs()
    {
        // Arrange
        var tourService = new MockTourService { ThrowException = true };
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.Equal("Error loading tours. Please try again.", model.Message);
    }

    [Fact]
    public async Task OnGetAsync_LogsError_WhenExceptionOccurs()
    {
        // Arrange
        var tourService = new MockTourService { ThrowException = true };
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.Equal(1, logger.LogErrorCallCount);
    }

    [Fact]
    public async Task OnGetAsync_DoesNotSetMessage_WhenNoException()
    {
        // Arrange
        var tourService = new MockTourService();
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.Null(model.Message);
    }

    [Fact]
    public void Tours_InitializesToEmptyList()
    {
        // Arrange
        var tourService = new MockTourService();
        var logger = new MockLogger();

        // Act
        var model = new IndexModel(tourService, logger);

        // Assert
        Assert.NotNull(model.Tours);
        Assert.Empty(model.Tours);
    }

    [Fact]
    public async Task OnGetAsync_PopulatesAllTourProperties()
    {
        // Arrange
        var tourService = new MockTourService();
        tourService.Tours.Add(new Tour
        {
            Id = 1,
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m
        });
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();

        // Assert
        var tour = model.Tours.First();
        Assert.Equal(1, tour.Id);
        Assert.Equal("Test Tour", tour.TourName);
        Assert.Equal("Paris", tour.Place);
        Assert.Equal(5, tour.Days);
        Assert.Equal(1000m, tour.Price);
    }

    [Fact]
    public async Task OnGetAsync_CanBeCalledMultipleTimes()
    {
        // Arrange
        var tourService = new MockTourService();
        tourService.Tours.Add(new Tour { Id = 1, TourName = "Tour 1" });
        var logger = new MockLogger();
        var model = new IndexModel(tourService, logger);

        // Act
        await model.OnGetAsync();
        await model.OnGetAsync();

        // Assert
        Assert.NotNull(model.Tours);
        Assert.Single(model.Tours);
    }
}
