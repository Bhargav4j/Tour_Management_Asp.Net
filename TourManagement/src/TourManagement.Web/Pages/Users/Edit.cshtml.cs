using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Users;

public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public UserInputModel UserInput { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var user = await _userService.GetByIdAsync(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            UserInput = new UserInputModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Street = user.Street,
                City = user.City,
                State = user.State
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit with ID: {UserId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingUser = await _userService.GetByIdAsync(UserInput.Id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Email = UserInput.Email;
            existingUser.FirstName = UserInput.FirstName;
            existingUser.LastName = UserInput.LastName;
            existingUser.Gender = UserInput.Gender;
            existingUser.DateOfBirth = UserInput.DateOfBirth;
            existingUser.Street = UserInput.Street;
            existingUser.City = UserInput.City;
            existingUser.State = UserInput.State;
            existingUser.ModifiedBy = "Admin";
            existingUser.ModifiedDate = DateTime.UtcNow;

            await _userService.UpdateAsync(UserInput.Id, existingUser);

            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
            return Page();
        }
    }

    public class UserInputModel
    {
        public int Id { get; set; }

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
