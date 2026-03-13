using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IUserService userService, ILogger<DetailsModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public new UserInfo? User { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            User = await _userService.GetByIdAsync(id.Value);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for ID: {UserId}", id);
            return RedirectToPage("./Index");
        }
    }
}
