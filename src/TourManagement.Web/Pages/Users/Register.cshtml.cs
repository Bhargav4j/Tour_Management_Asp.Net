using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class RegisterModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IUserInfoService userInfoService, ILogger<RegisterModel> logger)
    {
        _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime Dob { get; set; }

        [Required]
        [StringLength(50)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Check if user already exists
            var existingUser = await _userInfoService.GetByIdAsync(Input.Email, cancellationToken);
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Email", "An account with this email already exists.");
                return Page();
            }

            // Map input to entity
            var userInfo = new UserInfo
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State,
                CreatedBy = "system"
            };

            await _userInfoService.CreateAsync(userInfo, cancellationToken);

            _logger.LogInformation("New user registered: {Email}", Input.Email);
            TempData["SuccessMessage"] = "Registration successful! Please login.";

            return RedirectToPage("./Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
