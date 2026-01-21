using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class DetailsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IBookingService bookingService, ILogger<DetailsModel> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public BookingDetailsViewModel? Booking { get; set; }

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
            _logger.LogInformation("Retrieving booking details for ID: {BookingId}", id);
            var booking = await _bookingService.GetByIdAsync(id.Value, cancellationToken);

            if (booking == null)
            {
                ErrorMessage = $"Booking with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            Booking = new BookingDetailsViewModel
            {
                Id = booking.Id,
                TourId = booking.TourId,
                TourName = booking.Tour?.TourName ?? "N/A",
                TourPlace = booking.Tour?.Place ?? "N/A",
                TourDays = booking.Tour?.Days ?? 0,
                TourPrice = booking.Tour?.Price ?? 0,
                TourLocations = booking.Tour?.Locations ?? "N/A",
                TourInfo = booking.Tour?.TourInfo ?? "N/A",
                UserId = booking.UserId,
                UserName = booking.User?.Name ?? "N/A",
                UserEmail = booking.User?.Email ?? "N/A",
                UserPhoneNumber = booking.User?.PhoneNumber ?? "N/A",
                UserAddress = booking.User?.Address ?? "N/A",
                BookingDate = booking.BookingDate,
                NumberOfPeople = booking.NumberOfPeople,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,
                CreatedDate = booking.CreatedDate,
                ModifiedDate = booking.ModifiedDate,
                IsActive = booking.IsActive,
                CreatedBy = booking.CreatedBy,
                ModifiedBy = booking.ModifiedBy
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking details for ID: {BookingId}", id);
            ErrorMessage = "An error occurred while retrieving booking details. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public class BookingDetailsViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string TourPlace { get; set; } = string.Empty;
        public int TourDays { get; set; }
        public decimal TourPrice { get; set; }
        public string TourLocations { get; set; } = string.Empty;
        public string TourInfo { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhoneNumber { get; set; } = string.Empty;
        public string UserAddress { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
    }
}
