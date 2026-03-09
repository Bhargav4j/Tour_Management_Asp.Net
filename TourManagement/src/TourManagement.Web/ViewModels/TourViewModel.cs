using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

public class TourViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations are required")]
    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour information is required")]
    [StringLength(1000, ErrorMessage = "Tour information cannot exceed 1000 characters")]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Picture")]
    public string? PictureFileName { get; set; }

    [Display(Name = "Upload Picture")]
    public IFormFile? PictureFile { get; set; }

    public string? PictureUrl => !string.IsNullOrEmpty(PictureFileName)
        ? $"/uploads/tours/{PictureFileName}"
        : null;
}
