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
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (!string.IsNullOrEmpty(isAdmin))
            {
                // Admin can see all bookings
                Bookings = await _bookingService.GetAllAsync(cancellationToken);
                _logger.LogInformation("Admin retrieved all bookings");
            }
            else if (!string.IsNullOrEmpty(userEmail))
            {
                // Regular user sees only their bookings
                Bookings = await _bookingService.GetByUserEmailAsync(userEmail, cancellationToken);
                _logger.LogInformation("User {Email} retrieved their bookings", userEmail);
            }
            else
            {
                // No user logged in, show all bookings (or redirect to login)
                Bookings = await _bookingService.GetAllAsync(cancellationToken);
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            TempData["ErrorMessage"] = "An error occurred while retrieving bookings.";
            return Page();
        }
    }
}
