using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            Bookings = await _bookingService.GetByUserIdAsync(userId.Value);
            _logger.LogInformation("Retrieved {Count} bookings for user {UserId}", Bookings.Count(), userId.Value);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user {UserId}", userId.Value);
            Bookings = new List<BookingDto>();
            return Page();
        }
    }
}
