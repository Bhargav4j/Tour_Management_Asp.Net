using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class CreateBookingModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateBookingModel> _logger;

    public CreateBookingModel(IBookingService bookingService, ITourService tourService, ILogger<CreateBookingModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public int TourId { get; set; }

    public Tour? Tour { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? tourId)
    {
        if (tourId == null)
        {
            return NotFound();
        }

        TourId = tourId.Value;

        try
        {
            Tour = await _tourService.GetTourByIdAsync(TourId);

            if (Tour == null)
            {
                return NotFound();
            }

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
        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetTourByIdAsync(TourId);
            return Page();
        }

        try
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                ErrorMessage = "User not authenticated";
                Tour = await _tourService.GetTourByIdAsync(TourId);
                return Page();
            }

            var booking = new Booking
            {
                TourId = TourId,
                Email = userEmail
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created for tour {TourId} by user {Email}", TourId, userEmail);

            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourId}", TourId);
            ErrorMessage = "An error occurred while creating the booking. Please try again.";
            Tour = await _tourService.GetTourByIdAsync(TourId);
            return Page();
        }
    }
}
