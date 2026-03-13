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
    public Booking Booking { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var booking = await _bookingService.GetByIdAsync(id.Value);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for deletion with ID: {BookingId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _bookingService.DeleteAsync(Booking.Id);
            TempData["SuccessMessage"] = "Booking deleted successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", Booking.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the booking.");
            return Page();
        }
    }
}
