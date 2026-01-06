using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Bookings;

public class EditModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IBookingService bookingService, ILogger<EditModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public BookingInputModel BookingInput { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            BookingInput = new BookingInputModel
            {
                Id = booking.Id,
                TourName = booking.TourName,
                Place = booking.Place,
                Email = booking.Email,
                FirstName = booking.FirstName
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for editing, ID: {BookingId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var booking = new Booking
            {
                Id = BookingInput.Id,
                TourName = BookingInput.TourName,
                Place = BookingInput.Place,
                Email = BookingInput.Email,
                FirstName = BookingInput.FirstName
            };

            await _bookingService.UpdateBookingAsync(BookingInput.Id, booking);

            _logger.LogInformation("Updated booking with ID: {BookingId}", booking.Id);
            TempData["SuccessMessage"] = "Booking updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the booking.");
            return Page();
        }
    }

    public class BookingInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
    }
}
