using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

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
    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    [Phone]
    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    [BindProperty]
    [StringLength(500)]
    public string? Address { get; set; }

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
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Address = Address,
                CreatedBy = "Self"
            };

            await _userService.CreateAsync(user, Password);
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("./Login");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email {Email}", Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration.");
            return Page();
        }
    }
}
