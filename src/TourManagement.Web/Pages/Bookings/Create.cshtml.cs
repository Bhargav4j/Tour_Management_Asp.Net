using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public BookingInputModel BookingInput { get; set; } = new();

    public SelectList TourSelectList { get; set; } = new SelectList(new List<Tour>(), "TourId", "TourName");

    public class BookingInputModel
    {
        [Required]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken)
    {
        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            TourSelectList = new SelectList(tours, "TourId", "TourName");

            // Pre-populate user email if logged in
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (!string.IsNullOrEmpty(userEmail))
            {
                BookingInput.Email = userEmail;
            }

            // Pre-select tour if tourId is provided
            if (tourId.HasValue)
            {
                BookingInput.TourId = tourId.Value;
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking creation form");
            TempData["ErrorMessage"] = "An error occurred while loading the form.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            // Reload tours for the dropdown
            var tours = await _tourService.GetAllAsync(cancellationToken);
            TourSelectList = new SelectList(tours, "TourId", "TourName");
            return Page();
        }

        try
        {
            // Get tour details
            var tour = await _tourService.GetByIdAsync(BookingInput.TourId, cancellationToken);
            if (tour == null)
            {
                ModelState.AddModelError(string.Empty, "Selected tour not found.");
                return Page();
            }

            // Map input to entity
            var booking = new Booking
            {
                TourId = BookingInput.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Email = BookingInput.Email,
                FirstName = BookingInput.FirstName,
                CreatedBy = BookingInput.Email
            };

            await _bookingService.CreateAsync(booking, cancellationToken);

            _logger.LogInformation("Created new booking for user: {Email}, Tour: {TourId}", BookingInput.Email, BookingInput.TourId);
            TempData["SuccessMessage"] = "Booking created successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");

            // Reload tours for the dropdown
            var tours = await _tourService.GetAllAsync(cancellationToken);
            TourSelectList = new SelectList(tours, "TourId", "TourName");

            return Page();
        }
    }
}
