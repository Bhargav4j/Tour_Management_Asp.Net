using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

public class BookingViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "User is required")]
    [Display(Name = "User")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Tour is required")]
    [Display(Name = "Tour")]
    public int TourId { get; set; }

    [Required(ErrorMessage = "Booking date is required")]
    [DataType(DataType.Date)]
    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Number of people is required")]
    [Range(1, 100, ErrorMessage = "Number of people must be between 1 and 100")]
    [Display(Name = "Number of People")]
    public int NumberOfPeople { get; set; } = 1;

    [Display(Name = "Total Amount")]
    [DataType(DataType.Currency)]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public string? UserName { get; set; }
    public string? TourName { get; set; }
    public string? TourPlace { get; set; }
    public decimal? TourPrice { get; set; }
}
