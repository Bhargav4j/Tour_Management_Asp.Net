using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Users;

public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult OnGet()
    {
        // Get user info before clearing session
        var userEmail = HttpContext.Session.GetString("UserEmail");
        var userId = HttpContext.Session.GetInt32("UserId");

        // Clear all session data
        HttpContext.Session.Clear();

        _logger.LogInformation("User logged out: Email {Email}, ID {UserId}", userEmail, userId);

        return Page();
    }

    public IActionResult OnPost()
    {
        return OnGet();
    }
}
