using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for deleting a user
/// </summary>
public class DeleteModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IUserService userService, ILogger<DeleteModel> logger)
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
                return RedirectToPage("/Users/Login", new { returnUrl = $"/Users/Delete?id={id}" });
            }

            if (id == null)
            {
                _logger.LogWarning("Delete page accessed without ID");
                ErrorMessage = "User ID is required.";
                return RedirectToPage("Index");
            }

            // Prevent users from deleting themselves
            if (currentUserId == id)
            {
                _logger.LogWarning("User attempted to delete their own account. ID: {UserId}", id);
                ErrorMessage = "You cannot delete your own account.";
                return RedirectToPage("Index");
            }

            _logger.LogInformation("Loading user for deletion. ID: {UserId}", id.Value);

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

            _logger.LogInformation("Loaded user for deletion: {UserEmail}", user.Email);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for deletion. ID: {UserId}", id);
            ErrorMessage = "An error occurred while loading user data. Please try again.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int? id, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user is logged in
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Please login to access this page.";
                return RedirectToPage("/Users/Login");
            }

            if (id == null)
            {
                _logger.LogWarning("Delete POST without ID");
                ErrorMessage = "User ID is required.";
                return RedirectToPage("Index");
            }

            // Prevent users from deleting themselves
            if (currentUserId == id)
            {
                _logger.LogWarning("User attempted to delete their own account. ID: {UserId}", id);
                ErrorMessage = "You cannot delete your own account.";
                return RedirectToPage("Index");
            }

            _logger.LogInformation("Deleting user with ID: {UserId}", id.Value);

            // Get user details before deletion for logging
            var user = await _userService.GetByIdAsync(id.Value, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found during deletion. ID: {UserId}", id.Value);
                ErrorMessage = "User not found.";
                return RedirectToPage("Index");
            }

            var userEmail = user.Email;
            var bookingsCount = user.Bookings?.Count ?? 0;

            // Check if user has bookings
            if (bookingsCount > 0)
            {
                _logger.LogWarning("Attempted to delete user with {Count} bookings. ID: {UserId}", bookingsCount, id.Value);
                ErrorMessage = $"Cannot delete user with {bookingsCount} existing booking(s). Please reassign or delete the bookings first.";
                return RedirectToPage("Details", new { id = id.Value });
            }

            // Delete the user
            await _userService.DeleteAsync(id.Value, cancellationToken);

            _logger.LogInformation("User deleted successfully. Email: {UserEmail}, ID: {UserId}", userEmail, id.Value);

            TempData["SuccessMessage"] = $"User '{userEmail}' has been deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (NotFoundException nfex)
        {
            _logger.LogWarning(nfex, "User not found during deletion. ID: {UserId}", id);
            ErrorMessage = "User not found.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user. ID: {UserId}", id);
            ErrorMessage = "An error occurred while deleting the user. Please try again.";
            return RedirectToPage("Details", new { id = id });
        }
    }
}
