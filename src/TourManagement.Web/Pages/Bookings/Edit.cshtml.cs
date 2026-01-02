using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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
    public InputModel Input { get; set; } = new();

    public string UserName { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Number of People")]
        [Range(1, 100)]
        public int NumberOfPeople { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id, cancellationToken);

            if (booking == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                NumberOfPeople = booking.NumberOfPeople,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };

            UserName = booking.UserName;
            TourName = booking.TourName;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for edit with ID {BookingId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var dto = new BookingUpdateDto
            {
                BookingDate = Input.BookingDate,
                NumberOfPeople = Input.NumberOfPeople,
                TotalAmount = Input.TotalAmount,
                Status = Input.Status
            };

            await _bookingService.UpdateAsync(Input.Id, dto, cancellationToken);
            TempData["Message"] = "Booking updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the booking.");
            return Page();
        }
    }
}
