using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Tours;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Web.Pages.Tours;

public class ToursIndexModelTests
{
    private readonly Mock<ITourService> _mockTourService;
    private readonly Mock<ILogger<ToursIndexModel>> _mockLogger;

    public ToursIndexModelTests()
    {
        _mockTourService = new Mock<ITourService>();
        _mockLogger = new Mock<ILogger<ToursIndexModel>>();
    }

    [Fact]
    public void Constructor_WithNullTourService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ToursIndexModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ToursIndexModel(_mockTourService.Object, null!));
    }

    [Fact]
    public async Task OnGetAsync_WithoutSearchTerm_LoadsAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, Name = "Tour 1" },
            new Tour { Id = 2, Name = "Tour 2" }
        };
        _mockTourService.Setup(s => s.GetAllToursAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        var model = new ToursIndexModel(_mockTourService.Object, _mockLogger.Object);

        await model.OnGetAsync();

        Assert.Equal(2, model.Tours.Count());
    }

    [Fact]
    public async Task OnGetAsync_WithSearchTerm_SearchesTours()
    {
        var tours = new List<Tour> { new Tour { Id = 1, Name = "Paris Tour" } };
        _mockTourService.Setup(s => s.SearchToursAsync("Paris", It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        var model = new ToursIndexModel(_mockTourService.Object, _mockLogger.Object)
        {
            SearchTerm = "Paris"
        };

        await model.OnGetAsync();

        Assert.Single(model.Tours);
    }

    [Fact]
    public void Tours_InitializesAsEmptyList()
    {
        var model = new ToursIndexModel(_mockTourService.Object, _mockLogger.Object);

        Assert.NotNull(model.Tours);
        Assert.Empty(model.Tours);
    }
}
