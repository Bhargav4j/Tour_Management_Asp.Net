using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public EditInputModel? EditInput { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            ErrorMessage = "User ID is required.";
            return RedirectToPage("./Index");
        }

        try
        {
            var userDto = await _userService.GetByIdAsync(id.Value);

            if (userDto == null)
            {
                ErrorMessage = "User not found.";
                _logger.LogWarning("User not found with ID: {UserId}", id.Value);
                return RedirectToPage("./Index");
            }

            // Manual mapping from UserDto to EditInputModel
            EditInput = new EditInputModel
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Gender = userDto.Gender,
                DateOfBirth = userDto.DateOfBirth,
                Street = userDto.Street,
                City = userDto.City,
                State = userDto.State
            };

            _logger.LogInformation("Loaded edit form for user ID: {UserId}", id.Value);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for user ID: {UserId}", id.Value);
            ErrorMessage = "An error occurred while loading the edit form.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (EditInput == null)
        {
            ErrorMessage = "Invalid form data.";
            return Page();
        }

        try
        {
            // Check if user exists
            var existingUser = await _userService.GetByIdAsync(EditInput.Id);
            if (existingUser == null)
            {
                ErrorMessage = "User not found.";
                _logger.LogWarning("Attempted to update non-existent user ID: {UserId}", EditInput.Id);
                return RedirectToPage("./Index");
            }

            // Manual mapping from EditInputModel to UserUpdateDto
            var userUpdateDto = new UserUpdateDto
            {
                FirstName = EditInput.FirstName,
                LastName = EditInput.LastName,
                Gender = EditInput.Gender,
                DateOfBirth = EditInput.DateOfBirth,
                Street = EditInput.Street,
                City = EditInput.City,
                State = EditInput.State
            };

            // Update the user
            await _userService.UpdateAsync(EditInput.Id, userUpdateDto);

            _logger.LogInformation("User updated successfully: ID {UserId}", EditInput.Id);

            // Update session if the user is editing their own profile
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == EditInput.Id)
            {
                HttpContext.Session.SetString("UserName", $"{EditInput.FirstName} {EditInput.LastName}");
            }

            SuccessMessage = "User profile updated successfully!";
            return RedirectToPage("./Details", new { id = EditInput.Id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Update failed: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, ex.Message);
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", EditInput.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
            ErrorMessage = "An error occurred while updating the user.";
            return Page();
        }
    }

    public class EditInputModel
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Street address is required")]
        [StringLength(200, ErrorMessage = "Street address cannot exceed 200 characters")]
        [Display(Name = "Street Address")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        [Display(Name = "State")]
        public string State { get; set; } = string.Empty;
    }
}
