using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Pages.Bookings;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Web.Pages.Bookings;

public class BookingDetailsModelTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILogger<BookingDetailsModel>> _mockLogger;

    public BookingDetailsModelTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingDetailsModel>>();
    }

    [Fact]
    public void Constructor_WithNullBookingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingDetailsModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingDetailsModel(_mockBookingService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_LoadsBooking()
    {
        var booking = new Booking { Id = 1, TourName = "Test Booking" };
        _mockBookingService.Setup(s => s.GetBookingByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        var model = new BookingDetailsModel(_mockBookingService.Object, _mockLogger.Object);

        var result = await model.OnGetAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Booking);
        Assert.Equal("Test Booking", model.Booking.TourName);
    }

    [Fact]
    public async Task OnGetAsync_WithInvalidId_ReturnsNotFound()
    {
        _mockBookingService.Setup(s => s.GetBookingByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);
        var model = new BookingDetailsModel(_mockBookingService.Object, _mockLogger.Object);

        var result = await model.OnGetAsync(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
