using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IBookingService bookingService, ITourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public BookingViewModel Booking { get; set; } = new();

    public List<TourViewModel> AvailableTours { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            TempData["ErrorMessage"] = "Please login to create a booking.";
            return RedirectToPage("/Account/Login");
        }

        await LoadAvailableToursAsync();
        Booking.BookingDate = DateTime.Now;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            TempData["ErrorMessage"] = "Please login to create a booking.";
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            await LoadAvailableToursAsync();
            return Page();
        }

        try
        {
            Booking.UserId = userId.Value;
            var booking = MapToEntity(Booking);
            await _bookingService.CreateAsync(booking);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            await LoadAvailableToursAsync();
            return Page();
        }
    }

    private async Task LoadAvailableToursAsync()
    {
        try
        {
            var tours = await _tourService.GetAllAsync();
            AvailableTours = tours.Select(t => new TourViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Place = t.Place,
                Price = t.Price
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading available tours");
            AvailableTours = new List<TourViewModel>();
        }
    }

    private static Booking MapToEntity(BookingViewModel viewModel)
    {
        return new Booking
        {
            UserId = viewModel.UserId,
            TourId = viewModel.TourId,
            BookingDate = viewModel.BookingDate,
            NumberOfPeople = viewModel.NumberOfPeople,
            Status = "Pending",
            Notes = viewModel.Notes,
            CreatedBy = "User"
        };
    }
}
