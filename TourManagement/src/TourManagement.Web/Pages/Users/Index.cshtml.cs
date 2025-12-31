using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class UsersIndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersIndexModel> _logger;

    public UsersIndexModel(IUserService userService, ILogger<UsersIndexModel> logger)
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
            Users = new List<UserDto>();
        }
    }
}
