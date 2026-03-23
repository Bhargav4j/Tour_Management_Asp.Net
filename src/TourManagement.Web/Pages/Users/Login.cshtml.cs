using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class LoginModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserInfoService userInfoService, ILogger<LoginModel> logger)
    {
        _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

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
            var user = await _userInfoService.AuthenticateAsync(Input.Email, Input.Password, cancellationToken);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                _logger.LogWarning("Failed login attempt for email: {Email}", Input.Email);
                return Page();
            }

            // Set session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

            _logger.LogInformation("User logged in: {Email}", user.Email);
            TempData["SuccessMessage"] = $"Welcome back, {user.FirstName}!";

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }
}
