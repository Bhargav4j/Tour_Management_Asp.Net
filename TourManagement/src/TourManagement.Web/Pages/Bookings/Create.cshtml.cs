using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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
    public InputModel Input { get; set; } = new();

    public SelectList Users { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public SelectList Tours { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public class InputModel
    {
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required]
        [Range(1, 100)]
        [Display(Name = "Number of Persons")]
        public int NumberOfPersons { get; set; } = 1;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }

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
            var dto = new BookingCreateDto
            {
                UserId = Input.UserId,
                TourId = Input.TourId,
                BookingDate = Input.BookingDate,
                NumberOfPeople = Input.NumberOfPersons,
                TotalAmount = Input.TotalAmount,
                Status = "Pending"
            };

            await _bookingService.CreateAsync(dto);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "Error creating booking. Please try again.");
            await LoadDropdownsAsync();
            return Page();
        }
    }

    private async Task LoadDropdownsAsync()
    {
        var users = await _userService.GetAllAsync();
        Users = new SelectList(users, "Id", "Email");

        var tours = await _tourService.GetAllAsync();
        Tours = new SelectList(tours, "Id", "TourName");
    }
}
