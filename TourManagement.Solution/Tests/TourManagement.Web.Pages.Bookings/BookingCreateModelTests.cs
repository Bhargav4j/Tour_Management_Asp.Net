using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Bookings;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Tests.Web.Pages.Bookings;

public class BookingCreateModelTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ITourService> _mockTourService;
    private readonly Mock<ILogger<BookingCreateModel>> _mockLogger;

    public BookingCreateModelTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockTourService = new Mock<ITourService>();
        _mockLogger = new Mock<ILogger<BookingCreateModel>>();
    }

    [Fact]
    public void Constructor_WithNullBookingService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingCreateModel(null!, _mockTourService.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullTourService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingCreateModel(_mockBookingService.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BookingCreateModel(_mockBookingService.Object, _mockTourService.Object, null!));
    }

    [Fact]
    public void InputModel_InitializesWithDefaultValues()
    {
        var input = new BookingCreateModel.InputModel();

        Assert.Equal(0, input.TourId);
        Assert.Equal(string.Empty, input.TourName);
        Assert.Equal(string.Empty, input.Place);
        Assert.Equal(string.Empty, input.FirstName);
    }

    [Fact]
    public void InputModel_SetProperties_ShouldStoreValues()
    {
        var input = new BookingCreateModel.InputModel
        {
            TourId = 10,
            TourName = "Paris Tour",
            Place = "Paris",
            FirstName = "John"
        };

        Assert.Equal(10, input.TourId);
        Assert.Equal("Paris Tour", input.TourName);
        Assert.Equal("Paris", input.Place);
        Assert.Equal("John", input.FirstName);
    }
}
