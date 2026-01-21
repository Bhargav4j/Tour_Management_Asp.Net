using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for displaying user details
/// </summary>
public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IUserService userService, ILogger<DetailsModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public UserDetailsViewModel UserDetails { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user is logged in
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Please login to access this page.";
                return RedirectToPage("/Users/Login", new { returnUrl = $"/Users/Details?id={id}" });
            }

            if (id == null)
            {
                _logger.LogWarning("User details accessed without ID");
                ErrorMessage = "User ID is required.";
                return RedirectToPage("Index");
            }

            _logger.LogInformation("Loading details for user ID: {UserId}", id.Value);

            var user = await _userService.GetByIdAsync(id.Value, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id.Value);
                ErrorMessage = "User not found.";
                return RedirectToPage("Index");
            }

            // Map to view model
            UserDetails = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                IsActive = user.IsActive,
                CreatedBy = user.CreatedBy,
                ModifiedBy = user.ModifiedBy,
                BookingsCount = user.Bookings?.Count ?? 0
            };

            _logger.LogInformation("Loaded details for user: {UserEmail}", user.Email);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for ID: {UserId}", id);
            ErrorMessage = "An error occurred while loading user details. Please try again.";
            return RedirectToPage("Index");
        }
    }
}
