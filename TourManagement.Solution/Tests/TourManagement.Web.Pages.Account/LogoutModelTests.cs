using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Account;

namespace TourManagement.Tests.Web.Pages.Account;

public class LogoutModelTests
{
    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LogoutModel(null!));
    }

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        var mockLogger = new Mock<ILogger<LogoutModel>>();

        var model = new LogoutModel(mockLogger.Object);

        Assert.NotNull(model);
    }
}
