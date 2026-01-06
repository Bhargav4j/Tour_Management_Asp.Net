using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Loading bookings list");
            Bookings = await _bookingService.GetAllBookingsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            ModelState.AddModelError(string.Empty, "An error occurred while loading bookings.");
        }
    }
}
