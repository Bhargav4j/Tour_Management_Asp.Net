using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using TourManagement.Web.Pages;

namespace TourManagement.Tests.Web.Pages;

public class ErrorModelTests
{
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ErrorModel(null!));
    }

    [Fact]
    public void OnGet_SetsRequestId()
    {
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var model = new ErrorModel(mockLogger.Object);
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(c => c.TraceIdentifier).Returns("test-trace-id");
        model.PageContext.HttpContext = mockHttpContext.Object;

        model.OnGet();

        Assert.NotNull(model.RequestId);
        Assert.Equal("test-trace-id", model.RequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNotEmpty_ReturnsTrue()
    {
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var model = new ErrorModel(mockLogger.Object) { RequestId = "test-id" };

        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var model = new ErrorModel(mockLogger.Object) { RequestId = string.Empty };

        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        var mockLogger = new Mock<ILogger<ErrorModel>>();
        var model = new ErrorModel(mockLogger.Object) { RequestId = null };

        Assert.False(model.ShowRequestId);
    }
}
