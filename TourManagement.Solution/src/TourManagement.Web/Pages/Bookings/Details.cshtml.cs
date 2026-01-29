using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class BookingDetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingDetailsModel> _logger;

    public Booking? Booking { get; set; }

    public BookingDetailsModel(IBookingService bookingService, ILogger<BookingDetailsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Booking = await _bookingService.GetBookingByIdAsync(id);

            if (Booking == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking details for ID {BookingId}", id);
            return NotFound();
        }
    }
}
