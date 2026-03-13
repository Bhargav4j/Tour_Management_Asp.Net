using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly ITourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IBookingService bookingService,
        IUserService userService,
        ITourService tourService,
        ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _userService = userService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public BookingInputModel BookingInput { get; set; } = new();

    public SelectList UserList { get; set; } = null!;
    public SelectList TourList { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadDropdownsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        try
        {
            var booking = new Booking
            {
                UserId = BookingInput.UserId,
                TourId = BookingInput.TourId,
                NumberOfPeople = BookingInput.NumberOfPeople,
                TotalAmount = BookingInput.TotalAmount,
                BookingStatus = BookingInput.BookingStatus,
                CreatedBy = "Admin",
                CreatedDate = DateTime.UtcNow,
                BookingDate = DateTime.UtcNow,
                IsActive = true
            };

            await _bookingService.CreateAsync(booking);

            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            await LoadDropdownsAsync();
            return Page();
        }
    }

    private async Task LoadDropdownsAsync()
    {
        var users = await _userService.GetAllAsync();
        var tours = await _tourService.GetAllAsync();

        UserList = new SelectList(users, "Id", "Email");
        TourList = new SelectList(tours, "Id", "TourName");
    }

    public class BookingInputModel
    {
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Number of People")]
        public int NumberOfPeople { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Booking Status")]
        public string BookingStatus { get; set; } = "Pending";
    }
}
