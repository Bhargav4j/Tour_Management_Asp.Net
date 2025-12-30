using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<MyBookingViewModel> Bookings { get; set; } = new List<MyBookingViewModel>();

    [BindProperty(SupportsGet = true)]
    public string? UserEmail { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? UserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // If neither email nor userId provided, show empty page with search form
        if (string.IsNullOrWhiteSpace(UserEmail) && !UserId.HasValue)
        {
            Bookings = new List<MyBookingViewModel>();
            return Page();
        }

        try
        {
            IEnumerable<BookingDto> bookingDtos = new List<BookingDto>();

            // Fetch bookings by email or userId
            if (!string.IsNullOrWhiteSpace(UserEmail))
            {
                bookingDtos = await _bookingService.GetByUserEmailAsync(UserEmail);
                _logger.LogInformation("Retrieved bookings for email: {Email}", UserEmail);
            }
            else if (UserId.HasValue)
            {
                bookingDtos = await _bookingService.GetByUserIdAsync(UserId.Value);
                _logger.LogInformation("Retrieved bookings for user ID: {UserId}", UserId);
            }

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(StatusFilter))
            {
                bookingDtos = bookingDtos.Where(b =>
                    b.Status.Equals(StatusFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Manual mapping from BookingDto to MyBookingViewModel
            Bookings = bookingDtos
                .OrderByDescending(b => b.CreatedDate)
                .Select(dto => new MyBookingViewModel
                {
                    Id = dto.Id,
                    TourName = dto.TourName,
                    Place = dto.Place,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    TourId = dto.TourId,
                    UserId = dto.UserId,
                    BookingDate = dto.BookingDate,
                    Status = dto.Status,
                    CreatedDate = dto.CreatedDate,
                    ModifiedDate = dto.ModifiedDate,
                    IsActive = dto.IsActive
                })
                .ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user");
            TempData["ErrorMessage"] = "An error occurred while loading your bookings.";
            Bookings = new List<MyBookingViewModel>();
            return Page();
        }
    }

    public class MyBookingViewModel
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

        // Computed properties for display
        public string StatusBadgeClass => Status switch
        {
            "Confirmed" => "bg-success",
            "Cancelled" => "bg-danger",
            "Pending" => "bg-warning text-dark",
            "Completed" => "bg-info",
            _ => "bg-secondary"
        };

        public bool CanBeModified => Status != "Cancelled" && Status != "Completed";
    }
}
