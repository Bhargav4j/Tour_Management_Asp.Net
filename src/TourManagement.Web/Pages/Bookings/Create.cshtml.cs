using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

    [BindProperty(SupportsGet = true)]
    public int TourId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Number of people is required")]
    [Range(1, 10, ErrorMessage = "Number of people must be between 1 and 10")]
    public int NumberOfPeople { get; set; } = 1;

    public Tour? Tour { get; set; }

    public CreateBookingModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<CreateBookingModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
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
            _logger.LogError(ex, "Error loading tour {TourId} for booking", TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while loading tour details.");
            return Page();
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Account/Login");
            }

            Tour = await _tourService.GetTourByIdAsync(TourId);
            if (Tour == null)
            {
                return NotFound();
            }

            var booking = new Booking
            {
                UserId = userId,
                TourId = TourId,
                NumberOfPeople = NumberOfPeople
            };

            await _bookingService.CreateBookingAsync(booking);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("/Bookings/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour {TourId}", TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            Tour = await _tourService.GetTourByIdAsync(TourId);
            return Page();
        }
    }
}
