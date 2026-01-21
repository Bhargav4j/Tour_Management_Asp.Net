using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
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
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public CreateBookingViewModel Booking { get; set; } = new CreateBookingViewModel();

    public SelectList? Tours { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading Create Booking page");
            await LoadToursAsync(cancellationToken);

            // Pre-select tour if tourId is provided
            if (tourId.HasValue)
            {
                Booking.TourId = tourId.Value;
                await CalculateTotalAmountAsync(cancellationToken);
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Create Booking page");
            ErrorMessage = "An error occurred while loading the page. Please try again.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            await LoadToursAsync(cancellationToken);
            return Page();
        }

        try
        {
            _logger.LogInformation("Creating new booking for tour ID: {TourId}", Booking.TourId);

            // Get tour to calculate total amount
            var tour = await _tourService.GetByIdAsync(Booking.TourId, cancellationToken);
            if (tour == null)
            {
                ModelState.AddModelError(string.Empty, "Selected tour not found.");
                await LoadToursAsync(cancellationToken);
                return Page();
            }

            // Calculate total amount
            var totalAmount = tour.Price * Booking.NumberOfPeople;

            // Get userId from session (assuming user is logged in)
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue || userId.Value <= 0)
            {
                ModelState.AddModelError(string.Empty, "You must be logged in to create a booking. Please login first.");
                await LoadToursAsync(cancellationToken);
                return Page();
            }

            var booking = new Booking
            {
                TourId = Booking.TourId,
                UserId = userId.Value,
                BookingDate = Booking.BookingDate,
                NumberOfPeople = Booking.NumberOfPeople,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "System"
            };

            await _bookingService.CreateAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID: {BookingId}", booking.Id);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("./Index");
        }
        catch (TourManagement.Domain.Exceptions.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error creating booking");
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadToursAsync(cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            await LoadToursAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnGetCalculateTotalAsync(int tourId, int numberOfPeople, CancellationToken cancellationToken = default)
    {
        try
        {
            if (tourId <= 0 || numberOfPeople <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid parameters" });
            }

            var tour = await _tourService.GetByIdAsync(tourId, cancellationToken);
            if (tour == null)
            {
                return new JsonResult(new { success = false, message = "Tour not found" });
            }

            var totalAmount = tour.Price * numberOfPeople;
            return new JsonResult(new
            {
                success = true,
                totalAmount = totalAmount,
                pricePerPerson = tour.Price,
                numberOfPeople = numberOfPeople
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total amount");
            return new JsonResult(new { success = false, message = "Error calculating total" });
        }
    }

    private async Task LoadToursAsync(CancellationToken cancellationToken = default)
    {
        var tours = await _tourService.GetAllAsync(cancellationToken);
        var activeTours = tours.Where(t => t.IsActive).OrderBy(t => t.TourName);
        Tours = new SelectList(activeTours, nameof(Tour.Id), nameof(Tour.TourName));
    }

    private async Task CalculateTotalAmountAsync(CancellationToken cancellationToken = default)
    {
        if (Booking.TourId > 0 && Booking.NumberOfPeople > 0)
        {
            var tour = await _tourService.GetByIdAsync(Booking.TourId, cancellationToken);
            if (tour != null)
            {
                Booking.TotalAmount = tour.Price * Booking.NumberOfPeople;
            }
        }
    }

    public class CreateBookingViewModel
    {
        [Required(ErrorMessage = "Please select a tour")]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Number of people is required")]
        [Display(Name = "Number of People")]
        [Range(1, 100, ErrorMessage = "Number of people must be between 1 and 100")]
        public int NumberOfPeople { get; set; } = 1;

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }
    }
}
