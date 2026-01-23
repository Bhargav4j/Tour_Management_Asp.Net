using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Account;

public class RegisterPageModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterPageModel> _logger;

    public RegisterPageModel(IUserService userService, ILogger<RegisterPageModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public RegisterViewModel Input { get; set; } = new();

    [TempData]
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
            // Map ViewModel to DTO
            var createDto = new UserCreateDto
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                DateOfBirth = Input.DateOfBirth,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            await _userService.CreateAsync(createDto);

            _logger.LogInformation("User registered successfully: {Email}", Input.Email);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("/Account/Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", Input.Email);
            ErrorMessage = ex.Message;
            return Page();
        }
    }
}
