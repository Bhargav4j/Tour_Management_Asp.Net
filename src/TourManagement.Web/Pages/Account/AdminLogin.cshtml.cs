using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Pages.Account;

public class AdminLoginModel : PageModel
{
    private readonly ILogger<AdminLoginModel> _logger;
    private readonly IConfiguration _configuration;

    public AdminLoginModel(ILogger<AdminLoginModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [BindProperty]
    public AdminLoginInputModel Input { get; set; } = new AdminLoginInputModel();

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AdminCredentials:Password"] ?? "admin";

        if (Input.Email == adminEmail && Input.Password == adminPassword)
        {
            HttpContext.Session.SetString("UserEmail", Input.Email);
            HttpContext.Session.SetString("IsAdmin", "true");

            _logger.LogInformation("Admin {Email} logged in successfully", Input.Email);
            return RedirectToPage("/Index");
        }
        else
        {
            Message = "Invalid admin credentials.";
            IsSuccess = false;
            _logger.LogWarning("Failed admin login attempt for email {Email}", Input.Email);
            return Page();
        }
    }

    public class AdminLoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
