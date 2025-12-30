using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public BookingDeleteViewModel? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var bookingDto = await _bookingService.GetByIdAsync(id);

            if (bookingDto == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                return Page();
            }

            // Manual mapping from BookingDto to BookingDeleteViewModel
            Booking = new BookingDeleteViewModel
            {
                Id = bookingDto.Id,
                TourName = bookingDto.TourName,
                Place = bookingDto.Place,
                Email = bookingDto.Email,
                FirstName = bookingDto.FirstName,
                TourId = bookingDto.TourId,
                UserId = bookingDto.UserId,
                BookingDate = bookingDto.BookingDate,
                Status = bookingDto.Status,
                CreatedDate = bookingDto.CreatedDate,
                ModifiedDate = bookingDto.ModifiedDate,
                IsActive = bookingDto.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for deletion, ID: {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the booking.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Booking == null || Booking.Id == 0)
        {
            TempData["ErrorMessage"] = "Invalid booking ID.";
            return RedirectToPage("Index");
        }

        try
        {
            await _bookingService.DeleteAsync(Booking.Id);

            _logger.LogInformation("Booking {BookingId} deleted successfully", Booking.Id);
            TempData["SuccessMessage"] = $"Booking for {Booking.FirstName} has been deleted successfully.";

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId}", Booking.Id);
            TempData["ErrorMessage"] = "An error occurred while deleting the booking. Please try again.";
            return RedirectToPage("Details", new { id = Booking.Id });
        }
    }

    public class BookingDeleteViewModel
    {
        public int Id { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public int? TourId { get; set; }
        public int? UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
