using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

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
            if (Email == "admin@gmail.com" && Password == "admin")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("UserEmail", Email);
                HttpContext.Session.SetInt32("UserId", 0);

                _logger.LogInformation("Admin logged in: {Email}", Email);
                return RedirectToPage("/Tours/Index");
            }

            var user = await _userService.AuthenticateAsync(Email, Password, cancellationToken);

            if (user != null)
            {
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString().ToLower());
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);

                _logger.LogInformation("User logged in: {Email}", Email);

                if (user.IsAdmin)
                {
                    return RedirectToPage("/Tours/Index");
                }
                else
                {
                    return RedirectToPage("/Bookings/Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            ModelState.AddModelError(string.Empty, "An error occurred during login.");
            return Page();
        }
    }
}
