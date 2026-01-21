using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for editing user profile
/// </summary>
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public UserEditViewModel Input { get; set; } = new();

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
                return RedirectToPage("/Users/Login", new { returnUrl = $"/Users/Edit?id={id}" });
            }

            if (id == null)
            {
                _logger.LogWarning("Edit page accessed without ID");
                ErrorMessage = "User ID is required.";
                return RedirectToPage("Index");
            }

            _logger.LogInformation("Loading user for edit. ID: {UserId}", id.Value);

            var user = await _userService.GetByIdAsync(id.Value, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id.Value);
                ErrorMessage = "User not found.";
                return RedirectToPage("Index");
            }

            // Map to view model
            Input = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive
            };

            _logger.LogInformation("Loaded user for edit: {UserEmail}", user.Email);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit. ID: {UserId}", id);
            ErrorMessage = "An error occurred while loading user data. Please try again.";
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Updating user with ID: {UserId}", Input.Id);

            // Create user entity with updated data
            var user = new User
            {
                Id = Input.Id,
                Email = Input.Email,
                Name = Input.Name,
                PhoneNumber = Input.PhoneNumber,
                Address = Input.Address,
                IsActive = Input.IsActive
            };

            await _userService.UpdateAsync(Input.Id, user, cancellationToken);

            _logger.LogInformation("User updated successfully. ID: {UserId}", Input.Id);

            // Update session if current user edited their own profile
            if (currentUserId == Input.Id)
            {
                HttpContext.Session.SetString("UserEmail", Input.Email);
                HttpContext.Session.SetString("UserName", Input.Name);
            }

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToPage("Details", new { id = Input.Id });
        }
        catch (NotFoundException nfex)
        {
            _logger.LogWarning(nfex, "User not found during update. ID: {UserId}", Input.Id);
            ModelState.AddModelError(string.Empty, "User not found.");
            return Page();
        }
        catch (ValidationException vex)
        {
            _logger.LogWarning(vex, "Validation error during user update. ID: {UserId}", Input.Id);
            ModelState.AddModelError(string.Empty, vex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user. ID: {UserId}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user. Please try again.");
            return Page();
        }
    }
}
