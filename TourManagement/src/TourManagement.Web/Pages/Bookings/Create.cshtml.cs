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
    private readonly ILogger<BookingCreateModel> _logger;

    public BookingCreateModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<BookingCreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public int TourId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Booking date is required")]
    [DataType(DataType.Date)]
    public DateTime BookingDate { get; set; } = DateTime.Today;

    [BindProperty]
    [Required(ErrorMessage = "Number of people is required")]
    [Range(1, 50, ErrorMessage = "Number of people must be between 1 and 50")]
    public int NumberOfPeople { get; set; } = 1;

    public string TourName { get; set; } = string.Empty;
    public decimal TourPrice { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int tourId)
    {
        try
        {
            var tour = await _tourService.GetTourByIdAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            TourId = tourId;
            TourName = tour.Name;
            TourPrice = tour.Price;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking form for tour {TourId}", tourId);
            return RedirectToPage("/Tours/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var tour = await _tourService.GetTourByIdAsync(TourId);
            if (tour != null)
            {
                TourName = tour.Name;
                TourPrice = tour.Price;
            }
            return Page();
        }

        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                ErrorMessage = "Unable to identify user. Please log in again.";
                return Page();
            }

            var tour = await _tourService.GetTourByIdAsync(TourId);
            if (tour == null)
            {
                ErrorMessage = "Tour not found.";
                return Page();
            }

            var booking = new Booking
            {
                TourId = TourId,
                UserId = userId,
                BookingDate = BookingDate,
                NumberOfPeople = NumberOfPeople,
                TotalPrice = tour.Price * NumberOfPeople,
                Status = "Pending"
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created successfully for user {UserId}, tour {TourId}", userId, TourId);

            return RedirectToPage("/Bookings/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ErrorMessage = "An error occurred while creating the booking. Please try again.";

            var tour = await _tourService.GetTourByIdAsync(TourId);
            if (tour != null)
            {
                TourName = tour.Name;
                TourPrice = tour.Price;
            }

            return Page();
        }
    }
}
