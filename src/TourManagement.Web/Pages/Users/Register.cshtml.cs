using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user registration
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public RegisterViewModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Registration attempt for email: {Email}", Input.Email);

            // Check if email already exists
            var existingUser = await _userService.GetByEmailAsync(Input.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", Input.Email);
                ModelState.AddModelError("Input.Email", "An account with this email already exists.");
                return Page();
            }

            // Create new user
            var user = new User
            {
                Email = Input.Email,
                Name = Input.Name,
                PhoneNumber = Input.PhoneNumber,
                Address = Input.Address,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Registration"
            };

            var createdUser = await _userService.CreateAsync(user, Input.Password, cancellationToken);

            _logger.LogInformation("User registered successfully with ID: {UserId}", createdUser.Id);

            // Auto-login after registration
            HttpContext.Session.SetInt32("UserId", createdUser.Id);
            HttpContext.Session.SetString("UserEmail", createdUser.Email);
            HttpContext.Session.SetString("UserName", createdUser.Name);

            TempData["SuccessMessage"] = "Registration successful! Welcome to Tour Management.";
            return RedirectToPage("/Index");
        }
        catch (ValidationException vex)
        {
            _logger.LogWarning(vex, "Validation error during registration for email: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, vex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred while processing your registration. Please try again.");
            return Page();
        }
    }
}
