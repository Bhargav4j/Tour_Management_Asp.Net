using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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

    public BookingDto? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Booking = await _bookingService.GetByIdAsync(id, cancellationToken);

            if (Booking == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for cancel, id {BookingId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _bookingService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with id {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("Index");
        }
    }
}
