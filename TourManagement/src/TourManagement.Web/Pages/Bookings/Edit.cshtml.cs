using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

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
    public BookingInputModel BookingInput { get; set; } = new();

    public SelectList UserList { get; set; } = null!;
    public SelectList TourList { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var booking = await _bookingService.GetByIdAsync(id.Value);

            if (booking == null)
            {
                return NotFound();
            }

            BookingInput = new BookingInputModel
            {
                Id = booking.Id,
                UserId = booking.UserId,
                TourId = booking.TourId,
                NumberOfPeople = booking.NumberOfPeople,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus
            };

            await LoadDropdownsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for edit with ID: {BookingId}", id);
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
            var existingBooking = await _bookingService.GetByIdAsync(BookingInput.Id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            existingBooking.UserId = BookingInput.UserId;
            existingBooking.TourId = BookingInput.TourId;
            existingBooking.NumberOfPeople = BookingInput.NumberOfPeople;
            existingBooking.TotalAmount = BookingInput.TotalAmount;
            existingBooking.BookingStatus = BookingInput.BookingStatus;
            existingBooking.ModifiedBy = "Admin";
            existingBooking.ModifiedDate = DateTime.UtcNow;

            await _bookingService.UpdateAsync(BookingInput.Id, existingBooking);

            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the booking.");
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
        public int Id { get; set; }

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
