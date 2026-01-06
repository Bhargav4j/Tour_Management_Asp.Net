using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Tours;

namespace TourManagement.Web.Pages.Tours.Tests;

public class CreateModelTests
{
    private class MockTourService : ITourService
    {
        public List<Tour> Tours { get; set; } = new List<Tour>();
        public bool ThrowException { get; set; }

        public Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Tours.AsEnumerable());
        }

        public Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Tours.FirstOrDefault(t => t.Id == id));
        }

        public Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
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

    private class MockWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "/tmp/webroot";
        public string EnvironmentName { get; set; } = "Test";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = "/tmp";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }

    private class MockLogger : ILogger<CreateModel>
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
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();

        // Act
        var model = new CreateModel(tourService, environment, logger);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);

        // Act & Assert (should not throw)
        model.OnGet();
    }

    [Fact]
    public void Input_InitializesToNewInputModel()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();

        // Act
        var model = new CreateModel(tourService, environment, logger);

        // Assert
        Assert.NotNull(model.Input);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.ModelState.AddModelError("TourName", "Required");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_CreatesTour_WithValidInput()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Multiple",
            TourInfo = "Great tour"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Single(tourService.Tours);
        Assert.Equal("Test Tour", tourService.Tours.First().TourName);
    }

    [Fact]
    public async Task OnPostAsync_CreatesAndMapsData_OnSuccess()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Multiple",
            TourInfo = "Great tour"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Single(tourService.Tours);
        Assert.Equal("Test Tour", tourService.Tours.First().TourName);
    }

    [Fact]
    public async Task OnPostAsync_SetsErrorMessage_OnException()
    {
        // Arrange
        var tourService = new MockTourService { ThrowException = true };
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Multiple",
            TourInfo = "Great tour"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Equal("Error creating tour. Please try again.", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_LogsError_OnException()
    {
        // Arrange
        var tourService = new MockTourService { ThrowException = true };
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Multiple",
            TourInfo = "Great tour"
        };

        // Act
        await model.OnPostAsync();

        // Assert
        Assert.Equal(1, logger.LogErrorCallCount);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_OnException()
    {
        // Arrange
        var tourService = new MockTourService { ThrowException = true };
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1000m,
            Locations = "Multiple",
            TourInfo = "Great tour"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void InputModel_HasRequiredProperties()
    {
        // Arrange & Act
        var inputModel = new CreateModel.InputModel();

        // Assert
        Assert.NotNull(inputModel.TourName);
        Assert.NotNull(inputModel.Place);
        Assert.NotNull(inputModel.Locations);
        Assert.NotNull(inputModel.TourInfo);
    }

    [Fact]
    public void InputModel_CanSetAllProperties()
    {
        // Arrange
        var inputModel = new CreateModel.InputModel();

        // Act
        inputModel.TourName = "Tour Name";
        inputModel.Place = "Place";
        inputModel.Days = 7;
        inputModel.Price = 500m;
        inputModel.Locations = "Locations";
        inputModel.TourInfo = "Info";

        // Assert
        Assert.Equal("Tour Name", inputModel.TourName);
        Assert.Equal("Place", inputModel.Place);
        Assert.Equal(7, inputModel.Days);
        Assert.Equal(500m, inputModel.Price);
        Assert.Equal("Locations", inputModel.Locations);
        Assert.Equal("Info", inputModel.TourInfo);
    }

    [Fact]
    public async Task OnPostAsync_MapsTourProperties_Correctly()
    {
        // Arrange
        var tourService = new MockTourService();
        var environment = new MockWebHostEnvironment();
        var logger = new MockLogger();
        var model = new CreateModel(tourService, environment, logger);
        model.Input = new CreateModel.InputModel
        {
            TourName = "Mapped Tour",
            Place = "Mapped Place",
            Days = 10,
            Price = 1500.50m,
            Locations = "Mapped Locations",
            TourInfo = "Mapped Info"
        };

        // Act
        await model.OnPostAsync();

        // Assert
        var createdTour = tourService.Tours.First();
        Assert.Equal("Mapped Tour", createdTour.TourName);
        Assert.Equal("Mapped Place", createdTour.Place);
        Assert.Equal(10, createdTour.Days);
        Assert.Equal(1500.50m, createdTour.Price);
        Assert.Equal("Mapped Locations", createdTour.Locations);
        Assert.Equal("Mapped Info", createdTour.TourInfo);
    }
}
