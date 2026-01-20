using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserInfoService _userInfoService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserInfoService userInfoService, ILogger<LoginModel> logger)
    {
        _userInfoService = userInfoService;
        _logger = logger;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new LoginInputModel();

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
            var isValid = await _userInfoService.ValidateCredentialsAsync(Input.Email, Input.Password);

            if (isValid)
            {
                HttpContext.Session.SetString("UserEmail", Input.Email);
                HttpContext.Session.SetString("IsAdmin", "false");

                _logger.LogInformation("User {Email} logged in successfully", Input.Email);
                return RedirectToPage("/Index");
            }
            else
            {
                Message = "Invalid email or password.";
                IsSuccess = false;
                _logger.LogWarning("Failed login attempt for email {Email}", Input.Email);
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", Input.Email);
            Message = "An error occurred during login. Please try again.";
            IsSuccess = false;
            return Page();
        }
    }

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
