using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for listing all users (admin view)
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<User> Users { get; set; } = new List<User>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please login to access this page.";
                return RedirectToPage("/Users/Login", new { returnUrl = "/Users/Index" });
            }

            _logger.LogInformation("Loading users list. Search term: {SearchTerm}", SearchTerm ?? "None");

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Users = await _userService.SearchAsync(SearchTerm, cancellationToken);
                _logger.LogInformation("Found {Count} users matching search term: {SearchTerm}", Users.Count(), SearchTerm);
            }
            else
            {
                Users = await _userService.GetAllAsync(cancellationToken);
                _logger.LogInformation("Loaded {Count} total users", Users.Count());
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            ErrorMessage = "An error occurred while loading users. Please try again.";
            Users = new List<User>();
            return Page();
        }
    }
}
