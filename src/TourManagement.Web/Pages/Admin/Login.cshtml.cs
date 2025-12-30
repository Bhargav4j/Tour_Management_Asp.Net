using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        // Check if already logged in
        if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
        {
            Response.Redirect("/Admin/Dashboard");
            return;
        }

        // Check for logout message
        if (TempData["LogoutMessage"] != null)
        {
            SuccessMessage = TempData["LogoutMessage"]?.ToString();
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please correct the errors and try again.";
            return Page();
        }

        // Get admin credentials from configuration
        var adminEmail = _configuration["AdminCredentials:Email"];
        var adminPassword = _configuration["AdminCredentials:Password"];

        // Validate credentials
        if (Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) &&
            Password == adminPassword)
        {
            // Set session variables
            HttpContext.Session.SetString("AdminLoggedIn", "true");
            HttpContext.Session.SetString("AdminEmail", Email);
            HttpContext.Session.SetString("AdminLoginTime", DateTime.UtcNow.ToString("o"));

            // Set cookie if RememberMe is checked
            if (RememberMe)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("AdminRememberMe", "true", cookieOptions);
            }

            _logger.LogInformation("Admin user {Email} logged in successfully at {Time}",
                Email, DateTime.UtcNow);

            // Redirect to dashboard
            return RedirectToPage("/Admin/Dashboard");
        }
        else
        {
            _logger.LogWarning("Failed login attempt for admin user {Email} at {Time}",
                Email, DateTime.UtcNow);

            ErrorMessage = "Invalid email or password. Please try again.";
            return Page();
        }
    }
}
