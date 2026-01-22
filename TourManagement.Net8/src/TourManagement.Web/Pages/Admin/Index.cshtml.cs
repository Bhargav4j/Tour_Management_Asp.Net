using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var isAdmin = HttpContext.Session.GetString("IsAdmin");

        if (isAdmin != "True")
        {
            return RedirectToPage("/Account/Login");
        }

        return Page();
    }
}
