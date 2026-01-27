using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }

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
            var dto = new UserCreateDto
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                Phone = Input.Phone,
                Address = Input.Address,
                Role = Input.Role,
                IsActive = Input.IsActive
            };

            await _userService.CreateAsync(dto);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            ModelState.AddModelError(string.Empty, "Error creating user. Please try again.");
            return Page();
        }
    }
}
