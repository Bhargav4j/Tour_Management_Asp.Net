using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class BookingsIndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsIndexModel> _logger;

    public BookingsIndexModel(IBookingService bookingService, ILogger<BookingsIndexModel> logger)
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
            Bookings = new List<BookingDto>();
        }
    }
}
