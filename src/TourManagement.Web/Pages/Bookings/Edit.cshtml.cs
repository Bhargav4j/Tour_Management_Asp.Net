using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

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
    public BookingEditViewModel? Booking { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var bookingDto = await _bookingService.GetByIdAsync(id);

            if (bookingDto == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                return Page();
            }

            // Manual mapping from BookingDto to BookingEditViewModel
            Booking = new BookingEditViewModel
            {
                Id = bookingDto.Id,
                TourName = bookingDto.TourName,
                Place = bookingDto.Place,
                Email = bookingDto.Email,
                FirstName = bookingDto.FirstName,
                TourId = bookingDto.TourId,
                UserId = bookingDto.UserId,
                BookingDate = bookingDto.BookingDate,
                Status = bookingDto.Status,
                CreatedDate = bookingDto.CreatedDate,
                ModifiedDate = bookingDto.ModifiedDate,
                IsActive = bookingDto.IsActive
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for edit, ID: {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the booking.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Booking == null)
        {
            return Page();
        }

        try
        {
            // Manual mapping from BookingEditViewModel to BookingUpdateDto
            var updateDto = new BookingUpdateDto
            {
                FirstName = Booking.FirstName,
                Email = Booking.Email,
                TourName = Booking.TourName,
                Place = Booking.Place,
                Status = Booking.Status
            };

            await _bookingService.UpdateAsync(Booking.Id, updateDto);

            _logger.LogInformation("Booking {BookingId} updated successfully", Booking.Id);
            TempData["SuccessMessage"] = "Booking updated successfully!";

            return RedirectToPage("Details", new { id = Booking.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId}", Booking?.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the booking. Please try again.");
            return Page();
        }
    }

    public class BookingEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour name is required")]
        [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Place is required")]
        [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
        [Display(Name = "Place")]
        public string Place { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        // Read-only properties for display
        public int? TourId { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
