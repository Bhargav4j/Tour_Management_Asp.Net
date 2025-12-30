using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public List<UserViewModel> Users { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is admin (in a real application, you'd check roles/claims)
        var userRole = HttpContext.Session.GetString("UserRole");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            ErrorMessage = "Please login to access this page.";
            return RedirectToPage("./Login", new { returnUrl = "/Users/Index" });
        }

        // In a real application, you'd restrict this to admin users only
        // For now, we'll allow any logged-in user to view the list

        try
        {
            IEnumerable<UserDto> userDtos;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                userDtos = await _userService.SearchAsync(SearchTerm);
                _logger.LogInformation("Searched users with term: {SearchTerm}", SearchTerm);
            }
            else
            {
                userDtos = await _userService.GetAllAsync();
                _logger.LogInformation("Retrieved all users");
            }

            // Manual mapping from UserDto to UserViewModel
            Users = userDtos.Select(dto => new UserViewModel
            {
                Id = dto.Id,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = $"{dto.FirstName} {dto.LastName}",
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                CreatedDate = dto.CreatedDate,
                ModifiedDate = dto.ModifiedDate,
                IsActive = dto.IsActive
            }).ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            ErrorMessage = "An error occurred while retrieving users.";
            return Page();
        }
    }

    public class UserViewModel
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
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
