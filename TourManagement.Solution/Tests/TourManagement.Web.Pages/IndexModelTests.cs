using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace TourManagement.Tests.Web.Pages;

public class IndexModelTests
{
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new IndexModel(null!));
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        var mockLogger = new Mock<ILogger<IndexModel>>();
        var model = new IndexModel(mockLogger.Object);

        model.OnGet();

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Home page accessed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
