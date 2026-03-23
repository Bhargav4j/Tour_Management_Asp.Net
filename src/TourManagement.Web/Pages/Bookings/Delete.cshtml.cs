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
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public Booking? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Booking = await _bookingService.GetByIdAsync(id, cancellationToken);

            if (Booking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for delete: {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the booking.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Booking == null || Booking.BookingId == 0)
        {
            return NotFound();
        }

        try
        {
            await _bookingService.DeleteAsync(Booking.BookingId, cancellationToken);

            _logger.LogInformation("Cancelled booking: {BookingId}", Booking.BookingId);
            TempData["SuccessMessage"] = "Booking cancelled successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking: {BookingId}", Booking.BookingId);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("./Index");
        }
    }
}
