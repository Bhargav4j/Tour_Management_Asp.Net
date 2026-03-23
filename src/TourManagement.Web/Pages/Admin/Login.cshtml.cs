using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Simple hardcoded admin authentication (for demonstration)
        // In production, use proper authentication system
        if (Input.Username == "admin" && Input.Password == "admin123")
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            HttpContext.Session.SetString("AdminUsername", Input.Username);

            _logger.LogInformation("Admin logged in: {Username}", Input.Username);
            TempData["SuccessMessage"] = "Admin login successful!";

            return RedirectToPage("/Tours/Index");
        }

        ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
        _logger.LogWarning("Failed admin login attempt for username: {Username}", Input.Username);
        return Page();
    }
}
