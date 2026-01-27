using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

namespace TourManagement.Web.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public IEnumerable<UserDto> Users { get; set; } = new List<UserDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Users = await _userService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            ModelState.AddModelError(string.Empty, "Error loading users. Please try again.");
        }
    }
}
