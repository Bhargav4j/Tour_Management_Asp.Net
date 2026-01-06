using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using TourManagement.Web.Pages;

namespace TourManagement.Web.Pages.Tests;

public class ErrorModelTests
{
    private readonly Mock<ILogger<ErrorModel>> _mockLogger;

    public ErrorModelTests()
    {
        _mockLogger = new Mock<ILogger<ErrorModel>>();
    }

    [Fact]
    public void Constructor_InitializesLogger()
    {
        var model = new ErrorModel(_mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
    {
        var model = new ErrorModel(_mockLogger.Object);
        model.RequestId = null;
        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_ReturnsTrue_WhenRequestIdHasValue()
    {
        var model = new ErrorModel(_mockLogger.Object);
        model.RequestId = "test-request-id";
        Assert.True(model.ShowRequestId);
    }
}
