using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<BookingDto> Bookings { get; set; } = new List<BookingDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            Bookings = await _bookingService.GetByUserEmailAsync(userEmail, cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            ModelState.AddModelError(string.Empty, "An error occurred while loading bookings.");
            return Page();
        }
    }
}
