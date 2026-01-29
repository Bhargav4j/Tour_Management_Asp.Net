using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Bookings;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Web.Pages.Bookings;

public class BookingsIndexModelTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILogger<BookingsIndexModel>> _mockLogger;

    public BookingsIndexModelTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingsIndexModel>>();
    }

    [Fact]
    public void Constructor_WithNullBookingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingsIndexModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingsIndexModel(_mockBookingService.Object, null!));
    }

    [Fact]
    public void Bookings_InitializesAsEmptyList()
    {
        var model = new BookingsIndexModel(_mockBookingService.Object, _mockLogger.Object);

        Assert.NotNull(model.Bookings);
        Assert.Empty(model.Bookings);
    }
}
