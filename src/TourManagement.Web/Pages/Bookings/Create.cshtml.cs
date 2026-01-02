using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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
    public InputModel Input { get; set; } = new();

    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();
    public IEnumerable<TourDto> Tours { get; set; } = new List<TourDto>();

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
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Number of People")]
        [Range(1, 100)]
        public int NumberOfPeople { get; set; } = 1;

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Users = await _userService.GetAllAsync(cancellationToken);
        Tours = await _tourService.GetAllAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Users = await _userService.GetAllAsync(cancellationToken);
            Tours = await _tourService.GetAllAsync(cancellationToken);
            return Page();
        }

        try
        {
            var dto = new BookingCreateDto
            {
                UserId = Input.UserId,
                TourId = Input.TourId,
                BookingDate = Input.BookingDate,
                NumberOfPeople = Input.NumberOfPeople,
                TotalAmount = Input.TotalAmount
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            TempData["Message"] = "Booking created successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            Users = await _userService.GetAllAsync(cancellationToken);
            Tours = await _tourService.GetAllAsync(cancellationToken);
            return Page();
        }
    }
}
