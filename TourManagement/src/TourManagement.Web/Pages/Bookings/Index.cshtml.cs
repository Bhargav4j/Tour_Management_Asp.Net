using Microsoft.AspNetCore.Mvc;
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

    public IEnumerable<Booking>? Bookings { get; set; }

    public bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userId == null && userEmail == null)
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            if (IsAdmin)
            {
                Bookings = await _bookingService.GetAllBookingsAsync(cancellationToken);
            }
            else if (userId.HasValue)
            {
                Bookings = await _bookingService.GetBookingsByUserIdAsync(userId.Value, cancellationToken);
            }
            else
            {
                Bookings = Enumerable.Empty<Booking>();
            }

            _logger.LogInformation("Retrieved {Count} bookings", Bookings?.Count() ?? 0);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            Bookings = Enumerable.Empty<Booking>();
            return Page();
        }
    }
}
