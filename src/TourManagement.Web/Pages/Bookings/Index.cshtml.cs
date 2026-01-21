using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IList<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings for Index page");
            var bookings = await _bookingService.GetAllAsync(cancellationToken);

            Bookings = bookings.Select(b => new BookingViewModel
            {
                Id = b.Id,
                TourId = b.TourId,
                TourName = b.Tour?.TourName ?? "N/A",
                UserId = b.UserId,
                UserName = b.User?.Name ?? "N/A",
                UserEmail = b.User?.Email ?? "N/A",
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
            _logger.LogError(ex, "Error retrieving bookings for Index page");
            ErrorMessage = "An error occurred while retrieving bookings. Please try again.";
            return Page();
        }
    }

    public class BookingViewModel
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
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
