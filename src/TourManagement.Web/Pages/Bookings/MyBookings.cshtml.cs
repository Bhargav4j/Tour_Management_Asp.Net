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

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            Bookings = await _bookingService.GetBookingsByUserIdAsync(userId, cancellationToken);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            Bookings = new List<Booking>();
            return Page();
        }
    }
}
