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

    public CreateModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public BookingInputModel BookingInput { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? tourId)
    {
        if (tourId.HasValue)
        {
            var tour = await _tourService.GetTourByIdAsync(tourId.Value);
            if (tour != null)
            {
                BookingInput.TourName = tour.Name;
                BookingInput.Place = tour.Place;
            }
        }

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
            var booking = new Booking
            {
                TourName = BookingInput.TourName,
                Place = BookingInput.Place,
                Email = BookingInput.Email,
                FirstName = BookingInput.FirstName
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Created booking for {Email}", booking.Email);
            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            return Page();
        }
    }

    public class BookingInputModel
    {
        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
    }
}
