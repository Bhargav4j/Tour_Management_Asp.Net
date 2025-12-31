using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class CreateBookingModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateBookingModel> _logger;

    public CreateBookingModel(IBookingService bookingService, ILogger<CreateBookingModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel
    {
        [Required]
        public string TourName { get; set; } = string.Empty;

        [Required]
        public string Place { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int? TourId { get; set; }
    }

    public void OnGet(int? tourId)
    {
        if (tourId.HasValue)
        {
            Input.TourId = tourId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var createDto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                FirstName = Input.FirstName,
                Email = Input.Email,
                TourId = Input.TourId
            };

            await _bookingService.CreateAsync(createDto);
            _logger.LogInformation("Booking created for tour: {TourName}", Input.TourName);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            return Page();
        }
    }
}
