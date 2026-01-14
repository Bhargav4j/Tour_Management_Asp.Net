using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Account;

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
            var user = await _userService.ValidateLoginAsync(Email, Password, cancellationToken);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FirstName);

            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login.");
            return Page();
        }
    }
}
