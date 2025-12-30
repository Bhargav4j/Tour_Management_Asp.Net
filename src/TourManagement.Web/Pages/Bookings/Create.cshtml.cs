using System.ComponentModel.DataAnnotations;
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
        Input = new InputModel();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public Tour? Tour { get; set; }

    public class InputModel
    {
        [Required]
        public int TourId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Number of people must be between 1 and 100")]
        [Display(Name = "Number of People")]
        public int NumberOfPeople { get; set; } = 1;
    }

    public async Task<IActionResult> OnGetAsync(int tourId)
    {
        Input.TourId = tourId;
        Tour = await _tourService.GetTourByIdAsync(tourId);

        if (Tour == null)
        {
            return NotFound();
        }

        // Check if user is logged in
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Please log in to book a tour.";
            return RedirectToPage("/Users/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check if user is logged in
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Please log in to book a tour.";
            return RedirectToPage("/Users/Login");
        }

        Tour = await _tourService.GetTourByIdAsync(Input.TourId);
        if (Tour == null)
        {
            ModelState.AddModelError(string.Empty, "Tour not found.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var booking = new Booking
            {
                UserId = userId.Value,
                TourId = Input.TourId,
                NumberOfPeople = Input.NumberOfPeople,
                BookingDate = DateTime.UtcNow,
                CreatedBy = HttpContext.Session.GetString("UserEmail") ?? "User"
            };

            await _bookingService.CreateBookingAsync(booking);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("./MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {UserId} and tour {TourId}",
                userId.Value, Input.TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            return Page();
        }
    }
}
