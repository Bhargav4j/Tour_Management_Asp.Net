using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(userEmail))
            {
                Bookings = await _bookingService.GetUserBookingsAsync(userEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user bookings");
            ModelState.AddModelError(string.Empty, "Error loading bookings");
        }
    }
}
