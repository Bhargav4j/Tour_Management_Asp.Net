using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Tests.Pages;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;
    private readonly IndexModel _indexModel;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
        _indexModel = new IndexModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_indexModel);
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Act
        _indexModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OnGet_ShouldNotThrowException()
    {
        // Act & Assert
        var exception = Record.Exception(() => _indexModel.OnGet());
        Assert.Null(exception);
    }

    [Fact]
    public void OnGet_CalledMultipleTimes_ShouldLogMultipleTimes()
    {
        // Act
        _indexModel.OnGet();
        _indexModel.OnGet();
        _indexModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Home page accessed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
    }
}
