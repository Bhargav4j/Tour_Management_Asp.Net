using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IUserService userService, ILogger<DeleteModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public DeleteViewModel? User { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            ErrorMessage = "User ID is required.";
            return RedirectToPage("./Index");
        }

        try
        {
            var userDto = await _userService.GetByIdAsync(id.Value);

            if (userDto == null)
            {
                ErrorMessage = "User not found.";
                _logger.LogWarning("User not found with ID: {UserId}", id.Value);
                return RedirectToPage("./Index");
            }

            // Manual mapping from UserDto to DeleteViewModel
            User = new DeleteViewModel
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                Gender = userDto.Gender,
                DateOfBirth = userDto.DateOfBirth,
                Street = userDto.Street,
                City = userDto.City,
                State = userDto.State,
                FullAddress = $"{userDto.Street}, {userDto.City}, {userDto.State}",
                CreatedDate = userDto.CreatedDate,
                ModifiedDate = userDto.ModifiedDate,
                IsActive = userDto.IsActive
            };

            _logger.LogInformation("Loaded delete confirmation page for user ID: {UserId}", id.Value);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for user ID: {UserId}", id.Value);
            ErrorMessage = "An error occurred while loading the delete confirmation page.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User == null || User.Id <= 0)
        {
            ErrorMessage = "Invalid user data.";
            return RedirectToPage("./Index");
        }

        try
        {
            // Check if user is trying to delete their own account
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == User.Id)
            {
                ErrorMessage = "You cannot delete your own account while logged in.";
                _logger.LogWarning("User attempted to delete their own account: ID {UserId}", User.Id);
                return await OnGetAsync(User.Id);
            }

            // Verify user exists before deletion
            var existingUser = await _userService.GetByIdAsync(User.Id);
            if (existingUser == null)
            {
                ErrorMessage = "User not found. The user may have already been deleted.";
                _logger.LogWarning("Attempted to delete non-existent user ID: {UserId}", User.Id);
                return RedirectToPage("./Index");
            }

            // Delete the user
            await _userService.DeleteAsync(User.Id);

            _logger.LogInformation("User deleted successfully: ID {UserId}, Email {Email}", User.Id, User.Email);

            SuccessMessage = $"User '{User.FullName}' has been successfully deleted.";
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Delete operation failed: {Message}", ex.Message);
            ErrorMessage = ex.Message;
            return await OnGetAsync(User.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", User.Id);
            ErrorMessage = "An error occurred while deleting the user. Please try again.";
            return await OnGetAsync(User.Id);
        }
    }

    public class DeleteViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
