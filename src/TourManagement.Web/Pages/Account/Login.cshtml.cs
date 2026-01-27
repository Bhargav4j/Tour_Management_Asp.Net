using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

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

    public string? ErrorMessage { get; set; }

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
            var loginDto = new UserLoginDto
            {
                Email = Input.Email,
                Password = Input.Password
            };

            var user = await _userInfoService.ValidateLoginAsync(loginDto);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FirstName);

                _logger.LogInformation("User {Email} logged in successfully", user.Email);
                return RedirectToPage("/Tours/Index");
            }
            else
            {
                ErrorMessage = "Invalid email or password";
                _logger.LogWarning("Failed login attempt for {Email}", Input.Email);
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", Input.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
