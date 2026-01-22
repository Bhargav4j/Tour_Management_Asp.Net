using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    public string? FirstName { get; set; }

    [BindProperty]
    public string? LastName { get; set; }

    [BindProperty]
    public string? PhoneNumber { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingUser = await _userService.GetUserByEmailAsync(Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                return Page();
            }

            var user = new User
            {
                Email = Email,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                IsAdmin = false,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "System",
                IsActive = true
            };

            await _userService.CreateUserAsync(user);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("./Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
