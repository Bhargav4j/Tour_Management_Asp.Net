using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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

    public IEnumerable<BookingDto> Bookings { get; set; } = new List<BookingDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Bookings = await _bookingService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            ModelState.AddModelError(string.Empty, "Error loading bookings. Please try again.");
        }
    }
}
