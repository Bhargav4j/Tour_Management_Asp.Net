using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IList<MyBookingViewModel> Bookings { get; set; } = new List<MyBookingViewModel>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public string? UserName { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        // Get userId from session (assuming user is logged in)
        var userId = HttpContext.Session.GetInt32("UserId");
        UserName = HttpContext.Session.GetString("UserName");

        if (!userId.HasValue || userId.Value <= 0)
        {
            ErrorMessage = "You must be logged in to view your bookings. Please login first.";
            return RedirectToPage("/Users/Login");
        }

        try
        {
            _logger.LogInformation("Retrieving bookings for user ID: {UserId}", userId.Value);
            var bookings = await _bookingService.GetByUserIdAsync(userId.Value, cancellationToken);

            Bookings = bookings.Select(b => new MyBookingViewModel
            {
                Id = b.Id,
                TourId = b.TourId,
                TourName = b.Tour?.TourName ?? "N/A",
                TourPlace = b.Tour?.Place ?? "N/A",
                TourDays = b.Tour?.Days ?? 0,
                TourPrice = b.Tour?.Price ?? 0,
                TourPictureFileName = b.Tour?.PictureFileName,
                BookingDate = b.BookingDate,
                NumberOfPeople = b.NumberOfPeople,
                TotalAmount = b.TotalAmount,
                Status = b.Status,
                CreatedDate = b.CreatedDate,
                IsActive = b.IsActive
            }).OrderByDescending(b => b.CreatedDate).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user ID: {UserId}", userId.Value);
            ErrorMessage = "An error occurred while retrieving your bookings. Please try again.";
            return Page();
        }
    }

    public class MyBookingViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string TourPlace { get; set; } = string.Empty;
        public int TourDays { get; set; }
        public decimal TourPrice { get; set; }
        public string? TourPictureFileName { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
