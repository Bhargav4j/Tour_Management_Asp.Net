using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class CreateModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IUserService userService, ILogger<CreateModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserInputModel UserInput { get; set; } = new();

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
            var user = new UserInfo
            {
                Email = UserInput.Email,
                FirstName = UserInput.FirstName,
                LastName = UserInput.LastName,
                Gender = UserInput.Gender,
                Password = UserInput.Password,
                DateOfBirth = UserInput.DateOfBirth,
                Street = UserInput.Street,
                City = UserInput.City,
                State = UserInput.State,
                CreatedBy = "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _userService.CreateAsync(user);

            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the user.");
            return Page();
        }
    }

    public class UserInputModel
    {
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
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
