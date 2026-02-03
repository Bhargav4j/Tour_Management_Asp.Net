using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Pages.Tests;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _model;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _model = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldNotThrowException()
    {
        // Arrange, Act & Assert - Constructor does not validate null logger
        var model = new IndexModel(null!);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Arrange & Act
        var model = new IndexModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_ShouldExecuteWithoutErrors()
    {
        // Arrange & Act
        _model.OnGet();

        // Assert - Method completes without exceptions
        Assert.NotNull(_model);
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Arrange & Act
        _model.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
