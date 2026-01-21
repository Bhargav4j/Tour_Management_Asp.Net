using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public BookingDeleteViewModel Booking { get; set; } = new BookingDeleteViewModel();

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
            _logger.LogInformation("Loading Delete Booking page for ID: {BookingId}", id);
            var booking = await _bookingService.GetByIdAsync(id.Value, cancellationToken);

            if (booking == null)
            {
                ErrorMessage = $"Booking with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            Booking = new BookingDeleteViewModel
            {
                Id = booking.Id,
                TourId = booking.TourId,
                TourName = booking.Tour?.TourName ?? "N/A",
                TourPlace = booking.Tour?.Place ?? "N/A",
                TourDays = booking.Tour?.Days ?? 0,
                TourPrice = booking.Tour?.Price ?? 0,
                UserId = booking.UserId,
                UserName = booking.User?.Name ?? "N/A",
                UserEmail = booking.User?.Email ?? "N/A",
                BookingDate = booking.BookingDate,
                NumberOfPeople = booking.NumberOfPeople,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                CreatedDate = booking.CreatedDate,
                IsActive = booking.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Delete Booking page for ID: {BookingId}", id);
            ErrorMessage = "An error occurred while loading the booking. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (Booking.Id <= 0)
        {
            ErrorMessage = "Invalid booking ID.";
            return RedirectToPage("./Index");
        }

        try
        {
            _logger.LogInformation("Canceling/Deleting booking with ID: {BookingId}", Booking.Id);

            await _bookingService.DeleteAsync(Booking.Id, cancellationToken);
            _logger.LogInformation("Booking deleted successfully with ID: {BookingId}", Booking.Id);

            TempData["SuccessMessage"] = "Booking has been cancelled successfully!";
            return RedirectToPage("./Index");
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking not found for deletion: {BookingId}", Booking.Id);
            ErrorMessage = ex.Message;
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", Booking.Id);
            ErrorMessage = "An unexpected error occurred while canceling the booking. Please try again.";
            return Page();
        }
    }

    public class BookingDeleteViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string TourPlace { get; set; } = string.Empty;
        public int TourDays { get; set; }
        public decimal TourPrice { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
