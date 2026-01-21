using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// View model for editing user profile
/// </summary>
public class UserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
