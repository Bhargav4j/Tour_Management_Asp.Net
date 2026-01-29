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

    public Tour? Tour { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public BookingCreateModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<BookingCreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    public class InputModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int tourId)
    {
        try
        {
            Tour = await _tourService.GetTourByIdAsync(tourId);

            if (Tour == null)
            {
                return NotFound();
            }

            Input.TourId = Tour.Id;
            Input.TourName = Tour.Name;
            Input.Place = Tour.Place;
            Input.FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for booking");
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Tour = await _tourService.GetTourByIdAsync(Input.TourId);
            return Page();
        }

        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            var booking = new Booking
            {
                TourId = Input.TourId,
                TourName = Input.TourName,
                Place = Input.Place,
                UserEmail = userEmail,
                FirstName = Input.FirstName,
                CreatedBy = userEmail
            };

            await _bookingService.CreateBookingAsync(booking);

            _logger.LogInformation("Booking created for tour {TourId} by user {Email}", Input.TourId, userEmail);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("/Bookings/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking");
            Tour = await _tourService.GetTourByIdAsync(Input.TourId);
            return Page();
        }
    }
}
