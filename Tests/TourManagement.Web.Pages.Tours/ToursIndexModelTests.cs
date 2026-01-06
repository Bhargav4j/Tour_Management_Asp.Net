using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Tours;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace Tests.TourManagement.Web.Pages.Tours;

/// <summary>
/// Unit tests for Tours IndexModel
/// </summary>
public class ToursIndexModelTests
{
    private readonly Mock<ITourService> _mockTourService;
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _model;

    public ToursIndexModelTests()
    {
        _mockTourService = new Mock<ITourService>();
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _model = new IndexModel(_mockTourService.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange, Act & Assert
        Assert.NotNull(_model);
    }

    [Fact]
    public void Tours_InitializesAsEmptyList()
    {
        // Arrange, Act & Assert
        Assert.NotNull(_model.Tours);
        Assert.Empty(_model.Tours);
    }

    [Fact]
    public async Task OnGetAsync_LoadsTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, Name = "Tour 1", IsActive = true },
            new Tour { Id = 2, Name = "Tour 2", IsActive = true }
        };
        _mockTourService.Setup(s => s.GetAllToursAsync(default)).ReturnsAsync(tours);

        // Act
        await _model.OnGetAsync();

        // Assert
        Assert.NotNull(_model.Tours);
        Assert.Equal(2, _model.Tours.Count());
        _mockTourService.Verify(s => s.GetAllToursAsync(default), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_LogsInformation()
    {
        // Arrange
        _mockTourService.Setup(s => s.GetAllToursAsync(default))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _model.OnGetAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Loading tours list")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithException_LogsError()
    {
        // Arrange
        var exception = new Exception("Test exception");
        _mockTourService.Setup(s => s.GetAllToursAsync(default)).ThrowsAsync(exception);

        // Act
        await _model.OnGetAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error loading tours")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithException_AddsModelError()
    {
        // Arrange
        _mockTourService.Setup(s => s.GetAllToursAsync(default))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        await _model.OnGetAsync();

        // Assert
        Assert.False(_model.ModelState.IsValid);
        Assert.True(_model.ModelState.ContainsKey(string.Empty));
    }

    [Fact]
    public async Task OnGetAsync_WithEmptyResult_SetsEmptyTours()
    {
        // Arrange
        _mockTourService.Setup(s => s.GetAllToursAsync(default))
            .ReturnsAsync(new List<Tour>());

        // Act
        await _model.OnGetAsync();

        // Assert
        Assert.NotNull(_model.Tours);
        Assert.Empty(_model.Tours);
    }
}
