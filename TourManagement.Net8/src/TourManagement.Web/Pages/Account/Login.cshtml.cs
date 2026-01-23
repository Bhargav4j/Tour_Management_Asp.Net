using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Account;

public class LoginPageModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginPageModel> _logger;

    public LoginPageModel(IUserService userService, ILogger<LoginPageModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

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
            var isValid = await _userService.ValidateLoginAsync(Input.Email, Input.Password);

            if (isValid)
            {
                // Store user email in session
                HttpContext.Session.SetString("UserEmail", Input.Email);

                _logger.LogInformation("User {Email} logged in successfully", Input.Email);

                // Redirect to main profile page
                return RedirectToPage("/Account/Profile");
            }
            else
            {
                ErrorMessage = "Password is not correct";
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
}
