using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
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
    public int TourId { get; set; }

    [BindProperty]
    [Required]
    [Display(Name = "Booking Date")]
    [DataType(DataType.Date)]
    public DateTime BookingDate { get; set; } = DateTime.Today.AddDays(1);

    [BindProperty]
    [Required]
    [Range(1, 50)]
    [Display(Name = "Number of People")]
    public int NumberOfPeople { get; set; } = 1;

    [BindProperty]
    [Display(Name = "Additional Notes")]
    public string? Notes { get; set; }

    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken)
    {
        if (tourId == null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userId == null && userEmail == null)
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            Tour = await _tourService.GetTourByIdAsync(tourId.Value, cancellationToken);

            if (Tour == null)
            {
                return NotFound();
            }

            TourId = Tour.Id;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for booking");
            return RedirectToPage("/Tours/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            return Page();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (userId == null && userEmail == null)
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            if (tour == null)
            {
                return NotFound();
            }

            var booking = new Booking
            {
                TourId = TourId,
                UserId = userId ?? 1,
                BookingDate = BookingDate,
                NumberOfPeople = NumberOfPeople,
                Notes = Notes,
                Status = "Pending",
                CreatedBy = userEmail ?? "Guest",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _bookingService.CreateBookingAsync(booking, cancellationToken);

            _logger.LogInformation("Booking created for tour {TourId}", TourId);
            TempData["SuccessMessage"] = "Booking created successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            Tour = await _tourService.GetTourByIdAsync(TourId, cancellationToken);
            return Page();
        }
    }
}
