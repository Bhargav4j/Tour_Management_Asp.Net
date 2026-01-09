using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public Booking? Booking { get; set; }

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
                return NotFound();
            }

            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";
            if (!isAdmin && Booking.UserId != userId)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for deletion");
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Booking?.Id == null)
        {
            return NotFound();
        }

        try
        {
            await _bookingService.DeleteBookingAsync(Booking.Id, cancellationToken);

            _logger.LogInformation("Booking cancelled: {BookingId}", Booking.Id);
            TempData["SuccessMessage"] = "Booking cancelled successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            ModelState.AddModelError(string.Empty, "An error occurred while cancelling the booking.");
            return Page();
        }
    }
}
