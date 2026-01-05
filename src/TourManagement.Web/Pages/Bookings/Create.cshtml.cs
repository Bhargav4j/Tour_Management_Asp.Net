using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class CreateBookingModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateBookingModel> _logger;

    public CreateBookingModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<CreateBookingModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public TourDto? Tour { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Number of people is required")]
        [Range(1, 50, ErrorMessage = "Number of people must be between 1 and 50")]
        public int NumberOfPeople { get; set; } = 1;
    }

    public async Task<IActionResult> OnGetAsync(int? tourId)
    {
        if (!tourId.HasValue)
        {
            return RedirectToPage("/Tours/Index");
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            Tour = await _tourService.GetByIdAsync(tourId.Value);
            if (Tour == null)
            {
                return NotFound();
            }

            Input.TourId = tourId.Value;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for booking");
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetByIdAsync(Input.TourId);
            return Page();
        }

        try
        {
            var bookingCreateDto = new BookingCreateDto
            {
                UserId = userId.Value,
                TourId = Input.TourId,
                BookingDate = Input.BookingDate,
                NumberOfPeople = Input.NumberOfPeople
            };

            var booking = await _bookingService.CreateAsync(bookingCreateDto);

            _logger.LogInformation("Booking created: {BookingId}", booking.BookingId);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("/Bookings/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ErrorMessage = "An error occurred while creating the booking. Please try again.";
            Tour = await _tourService.GetByIdAsync(Input.TourId);
            return Page();
        }
    }
}
