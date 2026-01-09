using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public Booking? Booking { get; set; }

    public bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userId == null && userEmail == null)
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            Booking = await _bookingService.GetBookingByIdAsync(id.Value, cancellationToken);

            if (Booking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                return NotFound();
            }

            if (!IsAdmin && Booking.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access booking {BookingId}", userId, id);
                return RedirectToPage("./Index");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID {BookingId}", id);
            return RedirectToPage("./Index");
        }
    }
}
