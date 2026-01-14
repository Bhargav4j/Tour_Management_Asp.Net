using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.DTOs;
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
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Gender { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string Street { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string City { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [StringLength(50)]
    public string State { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var dto = new UserCreateDto
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                Password = Password,
                DateOfBirth = DateOfBirth,
                Street = Street,
                City = City,
                State = State
            };

            await _userService.CreateAsync(dto, cancellationToken);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Email may already be in use.");
            return Page();
        }
    }
}
