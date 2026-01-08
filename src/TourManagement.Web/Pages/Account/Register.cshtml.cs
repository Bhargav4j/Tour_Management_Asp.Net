using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Street is required")]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "City is required")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "State is required")]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

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
            var user = new User
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                DateOfBirth = DateOfBirth,
                Street = Street,
                City = City,
                State = State
            };

            await _userService.CreateUserAsync(user, Password, cancellationToken);

            _logger.LogInformation("User registered successfully: {Email}", Email);

            return RedirectToPage("/Account/Login");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", Email);
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", Email);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
