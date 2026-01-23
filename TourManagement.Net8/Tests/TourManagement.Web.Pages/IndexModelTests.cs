using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Pages.Tests;

public class IndexModelTests
{
    [Fact]
    public void Constructor_WithLogger_CreatesModel()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<IndexModel>>();

        // Act
        var model = new IndexModel(mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_LogsInformation()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<IndexModel>>();
        var model = new IndexModel(mockLogger.Object);

        // Act
        model.OnGet();

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
