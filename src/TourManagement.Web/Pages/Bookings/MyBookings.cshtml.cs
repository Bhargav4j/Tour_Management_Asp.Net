using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();

    public async Task OnGetAsync()
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                Bookings = await _bookingService.GetUserBookingsAsync(email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user bookings");
            Bookings = new List<Booking>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _bookingService.DeleteBookingAsync(id);
            _logger.LogInformation("Booking {BookingId} cancelled successfully", id);
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return RedirectToPage();
        }
    }
}
