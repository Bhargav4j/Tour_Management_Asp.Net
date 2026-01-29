using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Pages.Tours;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Web.Pages.Tours;

public class TourDetailsModelTests
{
    private readonly Mock<ITourService> _mockTourService;
    private readonly Mock<ILogger<TourDetailsModel>> _mockLogger;

    public TourDetailsModelTests()
    {
        _mockTourService = new Mock<ITourService>();
        _mockLogger = new Mock<ILogger<TourDetailsModel>>();
    }

    [Fact]
    public void Constructor_WithNullTourService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourDetailsModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TourDetailsModel(_mockTourService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithValidId_LoadsTour()
    {
        var tour = new Tour { Id = 1, Name = "Test Tour" };
        _mockTourService.Setup(s => s.GetTourByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var model = new TourDetailsModel(_mockTourService.Object, _mockLogger.Object);

        var result = await model.OnGetAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Tour);
        Assert.Equal("Test Tour", model.Tour.Name);
    }

    [Fact]
    public async Task OnGetAsync_WithInvalidId_ReturnsNotFound()
    {
        _mockTourService.Setup(s => s.GetTourByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var model = new TourDetailsModel(_mockTourService.Object, _mockLogger.Object);

        var result = await model.OnGetAsync(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
