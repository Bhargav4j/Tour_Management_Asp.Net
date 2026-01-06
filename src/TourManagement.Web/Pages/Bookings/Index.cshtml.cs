using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class BookingsIndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsIndexModel> _logger;

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public BookingsIndexModel(IBookingService bookingService, ILogger<BookingsIndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                Bookings = await _bookingService.GetUserBookingsAsync(userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user bookings");
            ModelState.AddModelError(string.Empty, "An error occurred while loading bookings.");
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null || booking.UserId != userId)
            {
                return NotFound();
            }

            await _bookingService.CancelBookingAsync(id);

            TempData["SuccessMessage"] = "Booking cancelled successfully.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage();
        }
    }
}
