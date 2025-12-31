using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly IUserService _userService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IBookingService bookingService,
        ITourService tourService,
        IUserService userService,
        ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _userService = userService;
        _logger = logger;
    }

    public Tour? Tour { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    [BindProperty]
    public int TourId { get; set; }

    public async Task<IActionResult> OnGetAsync(int tourId)
    {
        TourId = tourId;
        Tour = await _tourService.GetTourByIdAsync(tourId);

        if (Tour == null)
        {
            return NotFound();
        }

        UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Account/Login");
            }

            var tour = await _tourService.GetTourByIdAsync(TourId);
            var user = await _userService.GetUserByEmailAsync(email);

            if (tour == null || user == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to process booking.");
                return Page();
            }

            var booking = new Booking
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Email = user.Email,
                FirstName = user.FirstName
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created for tour {TourId} by user {Email}", tour.TourId, user.Email);

            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour {TourId}", TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            return Page();
        }
    }
}
