using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

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

    public List<BookingViewModel> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            IEnumerable<Booking> bookings;
            if (userId.HasValue)
            {
                bookings = await _bookingService.GetByUserIdAsync(userId.Value);
            }
            else
            {
                bookings = await _bookingService.GetAllAsync();
            }

            Bookings = bookings.Select(b => MapToViewModel(b)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings");
            Bookings = new List<BookingViewModel>();
        }
    }

    private static BookingViewModel MapToViewModel(Booking booking)
    {
        return new BookingViewModel
        {
            Id = booking.Id,
            UserId = booking.UserId,
            TourId = booking.TourId,
            BookingDate = booking.BookingDate,
            NumberOfPeople = booking.NumberOfPeople,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            Notes = booking.Notes,
            UserName = booking.User != null ? $"{booking.User.FirstName} {booking.User.LastName}" : null,
            TourName = booking.Tour?.Name,
            TourPlace = booking.Tour?.Place,
            TourPrice = booking.Tour?.Price
        };
    }
}
