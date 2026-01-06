using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;

namespace Tests.TourManagement.Web.Pages;

/// <summary>
/// Unit tests for PrivacyModel
/// </summary>
public class PrivacyModelTests
{
    private readonly Mock<ILogger<PrivacyModel>> _mockLogger;
    private readonly PrivacyModel _model;

    public PrivacyModelTests()
    {
        _mockLogger = new Mock<ILogger<PrivacyModel>>();
        _model = new PrivacyModel(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange, Act & Assert
        Assert.NotNull(_model);
    }

    [Fact]
    public void OnGet_ExecutesWithoutError()
    {
        // Arrange & Act
        _model.OnGet();

        // Assert - No exception thrown
        Assert.True(true);
    }
}
