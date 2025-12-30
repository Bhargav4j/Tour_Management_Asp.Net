using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public BookingDetailsViewModel? Booking { get; set; }

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

            // Manual mapping from BookingDto to BookingDetailsViewModel
            Booking = new BookingDetailsViewModel
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
            _logger.LogError(ex, "Error retrieving booking details for ID: {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the booking details.";
            return RedirectToPage("Index");
        }
    }

    public class BookingDetailsViewModel
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
