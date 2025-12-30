using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IUserService userService, ILogger<DetailsModel> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public UserDetailsViewModel? User { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

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

            // Manual mapping from UserDto to UserDetailsViewModel
            User = new UserDetailsViewModel
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                FullName = $"{userDto.FirstName} {userDto.LastName}",
                Gender = userDto.Gender,
                DateOfBirth = userDto.DateOfBirth,
                Age = CalculateAge(userDto.DateOfBirth),
                Street = userDto.Street,
                City = userDto.City,
                State = userDto.State,
                FullAddress = $"{userDto.Street}, {userDto.City}, {userDto.State}",
                CreatedDate = userDto.CreatedDate,
                ModifiedDate = userDto.ModifiedDate,
                IsActive = userDto.IsActive
            };

            _logger.LogInformation("Retrieved details for user ID: {UserId}", id.Value);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details for ID: {UserId}", id.Value);
            ErrorMessage = "An error occurred while retrieving user details.";
            return RedirectToPage("./Index");
        }
    }

    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
