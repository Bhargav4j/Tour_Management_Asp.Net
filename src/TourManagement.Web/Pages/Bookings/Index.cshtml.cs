using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

public class IndexModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IBookingService bookingService, ILogger<IndexModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            IEnumerable<BookingDto> bookingDtos;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                bookingDtos = await _bookingService.SearchAsync(SearchTerm);
            }
            else
            {
                bookingDtos = await _bookingService.GetAllAsync();
            }

            // Manual mapping from BookingDto to BookingViewModel
            Bookings = bookingDtos.Select(dto => new BookingViewModel
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
            }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            TempData["ErrorMessage"] = "An error occurred while loading bookings.";
            Bookings = new List<BookingViewModel>();
            return Page();
        }
    }

    public class BookingViewModel
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
