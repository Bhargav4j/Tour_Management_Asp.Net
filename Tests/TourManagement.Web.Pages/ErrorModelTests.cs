using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Web.Pages;
using System.Diagnostics;

namespace TourManagement.Web.Pages.Tests;

public class ErrorModelTests
{
    private readonly Mock<ILogger<ErrorModel>> _mockLogger;
    private readonly ErrorModel _model;

    public ErrorModelTests()
    {
        _mockLogger = new Mock<ILogger<ErrorModel>>();
        _model = new ErrorModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldNotThrowException()
    {
        // Arrange, Act & Assert - Constructor does not validate null logger
        var model = new ErrorModel(null!);
        Assert.NotNull(model);
    }

    [Fact]
    public void Constructor_WithValidLogger_ShouldCreateInstance()
    {
        // Arrange & Act
        var model = new ErrorModel(_mockLogger.Object);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void RequestId_DefaultValue_ShouldBeNull()
    {
        // Arrange & Act
        var model = new ErrorModel(_mockLogger.Object);

        // Assert
        Assert.Null(model.RequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ShouldReturnFalse()
    {
        // Arrange
        _model.RequestId = null;

        // Act
        var result = _model.ShowRequestId;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ShouldReturnFalse()
    {
        // Arrange
        _model.RequestId = string.Empty;

        // Act
        var result = _model.ShowRequestId;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ShouldReturnTrue()
    {
        // Arrange
        _model.RequestId = "test-request-id";

        // Act
        var result = _model.ShowRequestId;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OnGet_ShouldSetRequestIdFromHttpContext()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id";
        _model.PageContext = new PageContext
        {
            HttpContext = httpContext
        };

        // Act
        _model.OnGet();

        // Assert
        Assert.NotNull(_model.RequestId);
        Assert.Equal("test-trace-id", _model.RequestId);
    }

    [Fact]
    public void RequestId_SetValue_ShouldUpdateRequestId()
    {
        // Arrange
        var requestId = "custom-request-id";

        // Act
        _model.RequestId = requestId;

        // Assert
        Assert.Equal(requestId, _model.RequestId);
    }

    [Fact]
    public void ErrorModel_ShouldHaveResponseCacheAttribute()
    {
        // Arrange
        var type = typeof(ErrorModel);

        // Act
        var attributes = type.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

        // Assert
        Assert.NotEmpty(attributes);
        var attribute = attributes[0] as ResponseCacheAttribute;
        Assert.NotNull(attribute);
        Assert.Equal(0, attribute.Duration);
        Assert.Equal(ResponseCacheLocation.None, attribute.Location);
        Assert.True(attribute.NoStore);
    }

    [Fact]
    public void ErrorModel_ShouldHaveIgnoreAntiforgeryTokenAttribute()
    {
        // Arrange
        var type = typeof(ErrorModel);

        // Act
        var attributes = type.GetCustomAttributes(typeof(IgnoreAntiforgeryTokenAttribute), false);

        // Assert
        Assert.NotEmpty(attributes);
    }
}
