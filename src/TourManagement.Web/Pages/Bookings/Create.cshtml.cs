using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourManagement.Application.DTOs;
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
    public BookingInputModel Input { get; set; } = new();

    public SelectList ToursList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await LoadToursListAsync();

            // Set default booking date to today
            Input.BookingDate = DateTime.Now;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create booking page");
            TempData["ErrorMessage"] = "An error occurred while loading the page.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadToursListAsync();
            return Page();
        }

        try
        {
            // Manual mapping from InputModel to CreateDto
            var createDto = new BookingCreateDto
            {
                FirstName = Input.FirstName,
                Email = Input.Email,
                TourId = Input.TourId,
                UserId = Input.UserId
            };

            // If TourId is provided but TourName/Place are not, fetch from tour
            if (Input.TourId.HasValue)
            {
                var tour = await _tourService.GetByIdAsync(Input.TourId.Value);
                if (tour != null)
                {
                    createDto.TourName = string.IsNullOrWhiteSpace(Input.TourName) ? tour.TourName : Input.TourName;
                    createDto.Place = string.IsNullOrWhiteSpace(Input.Place) ? tour.Place : Input.Place;
                }
                else
                {
                    // Use provided values or defaults
                    createDto.TourName = Input.TourName ?? "Unknown Tour";
                    createDto.Place = Input.Place ?? "Unknown Place";
                }
            }
            else
            {
                // No tour selected, use provided values or defaults
                createDto.TourName = Input.TourName ?? "Custom Booking";
                createDto.Place = Input.Place ?? "Not Specified";
            }

            var createdBooking = await _bookingService.CreateAsync(createDto);

            _logger.LogInformation("Booking created successfully with ID: {BookingId}", createdBooking.Id);
            TempData["SuccessMessage"] = $"Booking created successfully for {Input.FirstName}!";

            return RedirectToPage("Details", new { id = createdBooking.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            await LoadToursListAsync();
            return Page();
        }
    }

    private async Task LoadToursListAsync()
    {
        try
        {
            var tours = await _tourService.GetAllAsync();
            var activeTours = tours.Where(t => t.IsActive).ToList();

            ToursList = new SelectList(
                activeTours.Select(t => new
                {
                    Value = t.Id,
                    Text = $"{t.TourName} - {t.Place} ({t.Days} days)"
                }),
                "Value",
                "Text"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            ToursList = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }

    public class BookingInputModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tour")]
        public int? TourId { get; set; }

        [Display(Name = "User ID")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        [Display(Name = "Tour Name")]
        public string? TourName { get; set; }

        [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
        [Display(Name = "Place")]
        public string? Place { get; set; }
    }
}
