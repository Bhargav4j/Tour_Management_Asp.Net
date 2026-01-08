using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

[Authorize]
public class BookingCreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly IUserService _userService;
    private readonly ILogger<BookingCreateModel> _logger;

    public BookingCreateModel(
        IBookingService bookingService,
        ITourService tourService,
        IUserService userService,
        ILogger<BookingCreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public int TourId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    public Tour? Tour { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int tourId, CancellationToken cancellationToken)
    {
        try
        {
            TourId = tourId;
            Tour = await _tourService.GetTourByIdAsync(tourId, cancellationToken);

            if (Tour == null)
            {
                return NotFound();
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var user = await _userService.GetUserByEmailAsync(userEmail, cancellationToken);
                if (user != null)
                {
                    Email = user.Email;
                    FirstName = user.FirstName;
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking page for tour: {TourId}", tourId);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            return Page();
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            if (tour == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                ErrorMessage = "User authentication error. Please login again.";
                Tour = tour;
                return Page();
            }

            var booking = new Booking
            {
                TourId = TourId,
                UserId = userId,
                TourName = tour.TourName,
                Place = tour.Place,
                Email = Email,
                FirstName = FirstName,
                BookingDate = DateTime.UtcNow
            };

            await _bookingService.CreateBookingAsync(booking, cancellationToken);

            _logger.LogInformation("Booking created successfully for tour: {TourId} by user: {UserId}", TourId, userId);

            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourId}", TourId);
            ErrorMessage = "An error occurred while creating the booking. Please try again.";
            Tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            return Page();
        }
    }
}
