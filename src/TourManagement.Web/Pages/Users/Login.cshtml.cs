using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user login
/// </summary>
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        Input.ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Login attempt for email: {Email}", Input.Email);

            var user = await _userService.AuthenticateAsync(Input.Email, Input.Password, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for email: {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact support.");
                return Page();
            }

            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.Name);

            _logger.LogInformation("User logged in successfully: {Email}", Input.Email);

            // Redirect to return URL or default page
            if (!string.IsNullOrEmpty(Input.ReturnUrl) && Url.IsLocalUrl(Input.ReturnUrl))
            {
                return LocalRedirect(Input.ReturnUrl);
            }

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
            return Page();
        }
    }

    public IActionResult OnPostLogout()
    {
        try
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            HttpContext.Session.Clear();

            _logger.LogInformation("User logged out: {Email}", userEmail);

            SuccessMessage = "You have been logged out successfully.";
            return RedirectToPage("/Users/Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            ErrorMessage = "An error occurred while logging out.";
            return RedirectToPage("/Users/Login");
        }
    }
}
