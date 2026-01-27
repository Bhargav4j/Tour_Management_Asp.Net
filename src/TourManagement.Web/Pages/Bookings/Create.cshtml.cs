using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public int TourId { get; set; }

    public Tour? Tour { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int tourId)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            Tour = await _tourService.GetTourByIdAsync(tourId);
            if (Tour == null)
            {
                return NotFound();
            }

            TourId = tourId;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for booking: {TourId}", tourId);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            var booking = new Booking
            {
                TourId = TourId,
                Email = email
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created for user {Email} and tour {TourId}", email, TourId);
            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ErrorMessage = ex.Message;
            Tour = await _tourService.GetTourByIdAsync(TourId);
            return Page();
        }
    }
}
