using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IUserService userService, ILogger<DeleteModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public new UserInfo User { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var user = await _userService.GetByIdAsync(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            User = user;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for deletion with ID: {UserId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _userService.DeleteAsync(User.Id);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", User.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the user.");
            return Page();
        }
    }
}
