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

    public BookingsIndexModel(IBookingService bookingService, ILogger<BookingsIndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<Booking>? Bookings { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                Bookings = await _bookingService.GetUserBookingsAsync(userId);
                _logger.LogInformation("Retrieved {Count} bookings for user {UserId}", Bookings?.Count() ?? 0, userId);
            }
            else
            {
                _logger.LogWarning("Could not parse user ID from claims");
                Bookings = new List<Booking>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            Bookings = new List<Booking>();
        }
    }
}
