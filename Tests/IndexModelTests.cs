using System;
using Xunit;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Pages.Tests;

public class IndexModelTests
{
    private class MockLogger : ILogger<IndexModel>
    {
        public int LogInformationCallCount { get; private set; }
        public string? LastLogMessage { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Information)
            {
                LogInformationCallCount++;
                LastLogMessage = formatter(state, exception);
            }
        }
    }

    [Fact]
    public void Constructor_InitializesModel()
    {
        // Arrange
        var logger = new MockLogger();

        // Act
        var model = new IndexModel(logger);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_AcceptsNullLogger()
    {
        // Act
        var model = new IndexModel(null!);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_LogsInformation()
    {
        // Arrange
        var logger = new MockLogger();
        var model = new IndexModel(logger);

        // Act
        model.OnGet();

        // Assert
        Assert.Equal(1, logger.LogInformationCallCount);
    }

    [Fact]
    public void OnGet_LogsCorrectMessage()
    {
        // Arrange
        var logger = new MockLogger();
        var model = new IndexModel(logger);

        // Act
        model.OnGet();

        // Assert
        Assert.Equal("Home page accessed", logger.LastLogMessage);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Arrange
        var logger = new MockLogger();
        var model = new IndexModel(logger);

        // Act & Assert (should not throw)
        model.OnGet();
    }

    [Fact]
    public void OnGet_CanBeCalledMultipleTimes()
    {
        // Arrange
        var logger = new MockLogger();
        var model = new IndexModel(logger);

        // Act
        model.OnGet();
        model.OnGet();
        model.OnGet();

        // Assert
        Assert.Equal(3, logger.LogInformationCallCount);
    }
}
