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
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

            _logger.LogInformation("Retrieved booking details for ID: {BookingId}", id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking details for ID: {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while retrieving booking details.";
            return RedirectToPage("./Index");
        }
    }
}
