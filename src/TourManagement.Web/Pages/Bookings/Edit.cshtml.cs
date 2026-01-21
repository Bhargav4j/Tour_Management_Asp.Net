using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class EditModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IBookingService bookingService,
        ITourService tourService,
        ILogger<EditModel> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public EditBookingViewModel Booking { get; set; } = new EditBookingViewModel();

    public SelectList? Statuses { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || id <= 0)
        {
            ErrorMessage = "Invalid booking ID.";
            return RedirectToPage("./Index");
        }

        try
        {
            _logger.LogInformation("Loading Edit Booking page for ID: {BookingId}", id);
            var booking = await _bookingService.GetByIdAsync(id.Value, cancellationToken);

            if (booking == null)
            {
                ErrorMessage = $"Booking with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            Booking = new EditBookingViewModel
            {
                Id = booking.Id,
                TourId = booking.TourId,
                TourName = booking.Tour?.TourName ?? "N/A",
                TourPrice = booking.Tour?.Price ?? 0,
                UserId = booking.UserId,
                UserName = booking.User?.Name ?? "N/A",
                BookingDate = booking.BookingDate,
                NumberOfPeople = booking.NumberOfPeople,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };

            LoadStatuses();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Edit Booking page for ID: {BookingId}", id);
            ErrorMessage = "An error occurred while loading the booking. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            LoadStatuses();
            return Page();
        }

        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", Booking.Id);

            // Get the existing booking
            var existingBooking = await _bookingService.GetByIdAsync(Booking.Id, cancellationToken);
            if (existingBooking == null)
            {
                ErrorMessage = $"Booking with ID {Booking.Id} not found.";
                return RedirectToPage("./Index");
            }

            // Get tour to recalculate total amount
            var tour = await _tourService.GetByIdAsync(existingBooking.TourId, cancellationToken);
            if (tour == null)
            {
                ModelState.AddModelError(string.Empty, "Associated tour not found.");
                LoadStatuses();
                return Page();
            }

            // Calculate new total amount
            var totalAmount = tour.Price * Booking.NumberOfPeople;

            // Create updated booking entity
            var updatedBooking = new Booking
            {
                Id = Booking.Id,
                TourId = existingBooking.TourId,
                UserId = existingBooking.UserId,
                BookingDate = Booking.BookingDate,
                NumberOfPeople = Booking.NumberOfPeople,
                TotalAmount = totalAmount,
                Status = Booking.Status,
                CreatedDate = existingBooking.CreatedDate,
                ModifiedDate = DateTime.UtcNow,
                IsActive = existingBooking.IsActive,
                CreatedBy = existingBooking.CreatedBy,
                ModifiedBy = "System"
            };

            await _bookingService.UpdateAsync(Booking.Id, updatedBooking, cancellationToken);
            _logger.LogInformation("Booking updated successfully with ID: {BookingId}", Booking.Id);

            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage("./Details", new { id = Booking.Id });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking not found for update: {BookingId}", Booking.Id);
            ErrorMessage = ex.Message;
            return RedirectToPage("./Index");
        }
        catch (TourManagement.Domain.Exceptions.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error updating booking");
            ModelState.AddModelError(string.Empty, ex.Message);
            LoadStatuses();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", Booking.Id);
            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            LoadStatuses();
            return Page();
        }
    }

    public async Task<IActionResult> OnGetCalculateTotalAsync(int numberOfPeople, CancellationToken cancellationToken = default)
    {
        try
        {
            if (numberOfPeople <= 0)
            {
                return new JsonResult(new { success = false, message = "Invalid number of people" });
            }

            // Get booking ID from query string
            var bookingId = int.Parse(Request.Query["id"].ToString());
            var booking = await _bookingService.GetByIdAsync(bookingId, cancellationToken);

            if (booking == null || booking.Tour == null)
            {
                return new JsonResult(new { success = false, message = "Booking or tour not found" });
            }

            var totalAmount = booking.Tour.Price * numberOfPeople;
            return new JsonResult(new
            {
                success = true,
                totalAmount = totalAmount,
                pricePerPerson = booking.Tour.Price,
                numberOfPeople = numberOfPeople
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total amount");
            return new JsonResult(new { success = false, message = "Error calculating total" });
        }
    }

    private void LoadStatuses()
    {
        var statuses = new List<string> { "Pending", "Confirmed", "Cancelled", "Completed" };
        Statuses = new SelectList(statuses);
    }

    public class EditBookingViewModel
    {
        public int Id { get; set; }

        public int TourId { get; set; }

        [Display(Name = "Tour")]
        public string TourName { get; set; } = string.Empty;

        public decimal TourPrice { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Customer")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Number of people is required")]
        [Display(Name = "Number of People")]
        [Range(1, 100, ErrorMessage = "Number of people must be between 1 and 100")]
        public int NumberOfPeople { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";
    }
}
