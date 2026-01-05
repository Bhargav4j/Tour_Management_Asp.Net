using Microsoft.AspNetCore.Authorization;
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
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var userEmail = User.Identity?.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                Bookings = await _bookingService.GetUserBookingsAsync(userEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user bookings");
            ErrorMessage = "An error occurred while loading your bookings. Please try again.";
        }
    }
}
