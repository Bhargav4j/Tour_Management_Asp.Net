using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// View model for tour create/edit operations
/// </summary>
public class TourViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(255)]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(255)]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations is required")]
    [StringLength(500)]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour info is required")]
    [StringLength(2000)]
    public string TourInfo { get; set; } = string.Empty;

    public string? PictureFileName { get; set; }

    public IFormFile? PictureFile { get; set; }
}
