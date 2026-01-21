using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// View model for Tour creation and editing
/// </summary>
public class TourViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(200, ErrorMessage = "Tour name cannot exceed 200 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(200, ErrorMessage = "Place cannot exceed 200 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Number of Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    [DataType(DataType.Currency)]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Locations cannot exceed 500 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Tour information cannot exceed 2000 characters")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Tour Information")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Tour Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Current Image")]
    public string? PictureFileName { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
