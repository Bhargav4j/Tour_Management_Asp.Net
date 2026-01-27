using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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
    public UserDto? User { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            User = await _userService.GetByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for deletion, ID: {UserId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User?.Id == null)
        {
            return NotFound();
        }

        try
        {
            await _userService.DeleteAsync(User.Id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user ID: {UserId}", User.Id);
            ModelState.AddModelError(string.Empty, "Error deleting user. Please try again.");
            return Page();
        }
    }
}
