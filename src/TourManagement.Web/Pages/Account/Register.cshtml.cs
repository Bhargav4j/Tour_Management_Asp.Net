using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IUserInfoService userInfoService, ILogger<RegisterModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new RegisterInputModel();

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingUser = await _userInfoService.GetUserByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                Message = "A user with this email already exists.";
                IsSuccess = false;
                return Page();
            }

            var userInfo = new UserInfo
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                DateOfBirth = Input.DateOfBirth,
                Street = Input.Street,
                City = Input.City,
                State = Input.State,
                CreatedBy = "Self",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _userInfoService.CreateUserAsync(userInfo);

            _logger.LogInformation("User registered successfully: {Email}", Input.Email);
            return RedirectToPage("./Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", Input.Email);
            Message = "An error occurred during registration. Please try again.";
            IsSuccess = false;
            return Page();
        }
    }

    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;
    }
}
