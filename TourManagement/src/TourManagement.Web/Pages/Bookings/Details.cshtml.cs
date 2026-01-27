using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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

    public BookingDto? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Booking = await _bookingService.GetByIdAsync(id);
            if (Booking == null)
            {
                return NotFound();
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID: {BookingId}", id);
            return RedirectToPage("./Index");
        }
    }
}
