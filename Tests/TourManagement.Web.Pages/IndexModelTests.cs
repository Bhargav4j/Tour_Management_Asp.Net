using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages;
using TourManagement.Domain.Entities;

namespace TourManagement.Web.Pages.Tests;

public class IndexModelTests
{
    private readonly Mock<ILogger<IndexModel>> _mockLogger;

    public IndexModelTests()
    {
        _mockLogger = new Mock<ILogger<IndexModel>>();
    }

    [Fact]
    public void Constructor_InitializesLogger()
    {
        var model = new IndexModel(_mockLogger.Object);
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_ExecutesWithoutException()
    {
        var model = new IndexModel(_mockLogger.Object);
        model.OnGet();
        Assert.True(true);
    }
}
