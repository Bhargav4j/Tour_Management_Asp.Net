using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

namespace TourManagement.Web.Pages.Bookings;

public class EditModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly ITourService _tourService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IBookingService bookingService,
        IUserService userService,
        ITourService tourService,
        ILogger<EditModel> logger)
    {
        _bookingService = bookingService;
        _userService = userService;
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Booking { get; set; } = new();

    public SelectList Users { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public SelectList Tours { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Tour")]
        public int TourId { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Number of Persons")]
        public int NumberOfPersons { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            Booking = new InputModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TourId = booking.TourId,
                BookingDate = booking.BookingDate,
                NumberOfPersons = booking.NumberOfPersons,
                TotalAmount = booking.TotalAmount,
                IsActive = booking.IsActive
            };

            await LoadDropdownsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for edit, ID: {BookingId}", id);
            return RedirectToPage("./Index");
        }
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
            var dto = new BookingUpdateDto
            {
                UserId = Booking.UserId,
                TourId = Booking.TourId,
                BookingDate = Booking.BookingDate,
                NumberOfPersons = Booking.NumberOfPersons,
                TotalAmount = Booking.TotalAmount,
                IsActive = Booking.IsActive
            };

            await _bookingService.UpdateAsync(Booking.Id, dto);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking ID: {BookingId}", Booking.Id);
            ModelState.AddModelError(string.Empty, "Error updating booking. Please try again.");
            await LoadDropdownsAsync();
            return Page();
        }
    }

    private async Task LoadDropdownsAsync()
    {
        var users = await _userService.GetAllAsync();
        Users = new SelectList(users, "Id", "Name");

        var tours = await _tourService.GetAllAsync();
        Tours = new SelectList(tours, "Id", "TourName");
    }
}
