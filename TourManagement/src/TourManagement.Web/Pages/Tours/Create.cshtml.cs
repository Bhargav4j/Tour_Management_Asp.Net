using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class CreateTourModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<CreateTourModel> _logger;

    public CreateTourModel(ITourService tourService, ILogger<CreateTourModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel
    {
        [Required]
        public string TourName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 365)]
        public int Duration { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
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
            var createDto = new TourCreateDto
            {
                TourName = Input.TourName,
                Description = Input.Description,
                Place = Input.Place,
                Price = Input.Price,
                Duration = Input.Duration,
                ImageUrl = Input.ImageUrl
            };

            await _tourService.CreateAsync(createDto);
            _logger.LogInformation("Tour created: {TourName}", Input.TourName);

            TempData["SuccessMessage"] = "Tour created successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour.");
            return Page();
        }
    }
}
