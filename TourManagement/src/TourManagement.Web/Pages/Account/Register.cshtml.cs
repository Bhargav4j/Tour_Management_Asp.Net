using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public RegisterViewModel RegisterViewModel { get; set; } = new();

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
            var user = MapToEntity(RegisterViewModel);
            await _userService.RegisterAsync(user, RegisterViewModel.Password);

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("Login");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", RegisterViewModel.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }

    private static User MapToEntity(RegisterViewModel viewModel)
    {
        return new User
        {
            Email = viewModel.Email,
            FirstName = viewModel.FirstName,
            LastName = viewModel.LastName,
            Gender = viewModel.Gender,
            DateOfBirth = viewModel.DateOfBirth,
            Street = viewModel.Street,
            City = viewModel.City,
            State = viewModel.State,
            CreatedBy = "System"
        };
    }
}
