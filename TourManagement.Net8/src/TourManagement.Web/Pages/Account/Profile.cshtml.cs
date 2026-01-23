using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Account;

public class ProfilePageModel : PageModel
{
    private readonly ILogger<ProfilePageModel> _logger;

    public ProfilePageModel(ILogger<ProfilePageModel> logger)
    {
        _logger = logger;
    }

    public string? UserEmail { get; set; }

    public IActionResult OnGet()
    {
        // Check if user is logged in
        UserEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(UserEmail))
        {
            return RedirectToPage("/Account/Login");
        }

        return Page();
    }
}
